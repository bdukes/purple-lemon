using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Miniblog.Core.Models
{
    using System.Linq;
    using System.Xml;

    using Microsoft.AspNetCore.Html;

    public class Post : IEquatable<Post>
    {
        [Required]
        public string ID { get; set; } = DateTime.UtcNow.Ticks.ToString();

        [Required]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Excerpt { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; } = true;

        public IList<string> Categories { get; set; } = new List<string>();

        public IList<Comment> Comments { get; } = new List<Comment>();

        public string GetLink()
        {
            return $"/blog/{Slug}/";
        }

        public bool AreCommentsOpen(int commentsCloseAfterDays)
        {
            return PubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;
        }

        public static string CreateSlug(string title)
        {
            title = title.ToLowerInvariant().Replace(" ", "-");
            title = RemoveDiacritics(title);
            title = RemoveReservedUrlCharacters(title);

            return title.ToLowerInvariant();
        }

        static string RemoveReservedUrlCharacters(string text)
        {
            var reservedCharacters = new List<string> { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

            foreach (var chr in reservedCharacters)
            {
                text = text.Replace(chr, "");
            }

            return text;
        }

        static string RemoveDiacritics(string text)
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

        public string RenderContent()
        {
            var result = Content;
            if (!string.IsNullOrEmpty(result))
            {
                // Set up lazy loading of images/iframes
                result = result.Replace(" src=\"", " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"");

                // Youtube content embedded using this syntax: [youtube:xyzAbc123]
                var video = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
                result = Regex.Replace(result, @"\[youtube:(.*?)\]", m => string.Format(video, m.Groups[1].Value));
            }
            return result;
        }

        public HtmlString FindPrimaryImageTag()
        {
            var imgRegex = new Regex("<img[^>].+ />", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return (from Match match in imgRegex.Matches(this.Content)
                    let img = LoadXml(match.Value)
                    let srcNode = img.Attributes["src"]
                    where !string.IsNullOrWhiteSpace(srcNode?.Value)
                    let widthNode = img.Attributes["width"]
                    let heightNode = img.Attributes["height"]
                    let width = TryParseInt32(widthNode?.Value)
                    let height = TryParseInt32(heightNode?.Value)
                    let hasDimensions = width != null && height != null
                    let isLandscape = hasDimensions && width > height
                    orderby isLandscape ? 0 : 1
                    select new HtmlString(match.Value))
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
    }
}
