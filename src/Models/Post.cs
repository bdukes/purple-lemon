namespace Miniblog.Core.Models
{
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    using Microsoft.AspNetCore.Html;

    public class Post : IEquatable<Post>
    {
        public IList<string> Categories { get; } = new List<string>();

        public IList<Comment> Comments { get; } = new List<Comment>();

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string Excerpt { get; set; } = string.Empty;

        [Required]
        public string ID { get; set; } = DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture);

        public bool IsPublished { get; set; } = true;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "The slug should be lower case.")]
        public static string CreateSlug(string title)
        {
            title = title?.ToLowerInvariant().Replace(
                Constants.Space, Constants.Dash, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
            title = RemoveDiacritics(title);
            title = RemoveReservedUrlCharacters(title);

            return title.ToLowerInvariant();
        }

        public bool AreCommentsOpen(int commentsCloseAfterDays) =>
            this.PubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;

        public string GetEncodedLink() => $"/blog/{System.Net.WebUtility.UrlEncode(this.Slug)}/";

        public string GetLink() => $"/blog/{this.Slug}/";

        public bool IsVisible() => this.PubDate <= DateTime.UtcNow && this.IsPublished;

        public string RenderContent()
        {
            var result = this.Content;

            // Set up lazy loading of images/iframes
            if (!string.IsNullOrEmpty(result))
            {
                // Set up lazy loading of images/iframes
                var replacement = " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"";
                var pattern = "(<img.*?)(src=[\\\"|'])(?<src>.*?)([\\\"|'].*?[/]?>)";
                result = Regex.Replace(result, pattern, m => m.Groups[1].Value + replacement + m.Groups[4].Value + m.Groups[3].Value);

                // Youtube content embedded using this syntax: [youtube:xyzAbc123]
                var video = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
                result = Regex.Replace(
                    result,
                    @"\[youtube:(.*?)\]",
                    m => string.Format(CultureInfo.InvariantCulture, video, m.Groups[1].Value));
            }

            return result;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private static string RemoveReservedUrlCharacters(string text)
        {
            var reservedCharacters = new List<string> { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

            foreach (var chr in reservedCharacters)
            {
                text = text.Replace(chr, string.Empty, StringComparison.OrdinalIgnoreCase);
            }

            return text;
        }

        public (string url, string height, string width) FindPrimaryImageDetails()
        {
            var imageNode = this.FindPrimaryImageNode();
            if (imageNode == null)
            {
                return default((string url, string height, string width));
            }

            var src = imageNode.Attributes["src"]?.Value;
            var height = imageNode.Attributes["height"]?.Value;
            var width = imageNode.Attributes["width"]?.Value;
            return (src, height, width);
        }

        public HtmlString FindPrimaryImageTag(int? preferredWidth, int? preferredHeight)
        {
            var img = FindPrimaryImageNode();
            if (preferredWidth != null && preferredHeight != null)
            {
                string width = preferredWidth.Value.ToString(CultureInfo.InvariantCulture);
                string height = preferredHeight.Value.ToString(CultureInfo.InvariantCulture);
                string src = Regex.Replace(
                    img.Attributes["src"].Value,
                    @"/([^/]*f_auto[^/]*)/",
                    $"/$1,c_fill,w_{width},h_{height}/");
                img.Attributes["src"].Value = src;
                img.Attributes["width"].Value = width;
                img.Attributes["height"].Value = height;
            }

            using (var stringWriter = new StringWriter())
            using (var textWriter = new System.Xml.XmlTextWriter(stringWriter))
            {
                img.WriteTo(textWriter);

                return new HtmlString(stringWriter.ToString());
            }
        }

        public bool Equals(Post other)
        {
            return this.ID.Equals(other.ID, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is Post post && this.Equals(post);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        private XmlNode FindPrimaryImageNode()
        {
            var imgRegex = new Regex("<img [^>]+/>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return (from Match match in imgRegex.Matches(this.Content)
                    let img = LoadXml(match.Value)
                    let srcNode = img.Attributes["src"]
                    where !string.IsNullOrWhiteSpace(srcNode?.Value)
                    let classNode = img.Attributes["class"]
                    let classes = classNode?.Value?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? new string[0]
                    let isPrimary = classes.Contains("image__primary", StringComparer.OrdinalIgnoreCase)
                    let widthNode = img.Attributes["width"]
                    let heightNode = img.Attributes["height"]
                    let width = TryParseInt32(widthNode?.Value)
                    let height = TryParseInt32(heightNode?.Value)
                    let hasDimensions = width != null && height != null
                    let isLandscape = hasDimensions && width > height
                    orderby isPrimary ? 0 : isLandscape ? 1 : 2
                    select img)
                .FirstOrDefault();

            XmlNode LoadXml(string xml)
            {
                var doc = new XmlDocument();
                doc.LoadXml("<root>" + xml + "</root>");

                return doc.FirstChild.FirstChild;
            }

            int? TryParseInt32(string val)
            {
                return int.TryParse(val, out var result) ? result : default(int?);
            }
        }
    }
}
