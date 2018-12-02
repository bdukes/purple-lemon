namespace Miniblog.Core.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    public class CloudinaryFileBlogService : FileBlogService, IBlogService
    {
        private readonly Cloudinary cloudinary = new Cloudinary();

        public CloudinaryFileBlogService(IHostingEnvironment env, IHttpContextAccessor contextAccessor)
            : base(env, contextAccessor)
        {
        }

        Task<string> IBlogService.SaveFile(byte[] bytes, string fileName, string suffix)
        {
            string ext = Path.GetExtension(fileName);
            string name = Path.GetFileNameWithoutExtension(fileName);

            string fileNameWithSuffix = $"{name}_{suffix}{ext}";

            var uploadResult = this.cloudinary.Upload(
                new ImageUploadParams
                    {
                        File = new FileDescription(fileNameWithSuffix, new MemoryStream(bytes)),
                        UseFilename = true,
                        Folder = "Posts/Files",
                    });

            if (!string.IsNullOrEmpty(uploadResult.Error?.Message))
            {
                throw new InvalidOperationException(uploadResult.Error.Message);
            }

            var url = this.cloudinary.Api.Url
                .Secure(true)
                .Action("upload")
                .ResourceType(uploadResult.ResourceType)
                .UseRootPath(this.cloudinary.Api.UseRootPath)
                .Suffix(this.cloudinary.Api.Suffix)
                .Version(uploadResult.Version)
                .Transform(new Transformation().FetchFormat("auto").Quality("auto").Flags("lossy", "keep_attribution"))
                .BuildUrl(uploadResult.PublicId);
            return Task.FromResult(url);
        }
    }
}
