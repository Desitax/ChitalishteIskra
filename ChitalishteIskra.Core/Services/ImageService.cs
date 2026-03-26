using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Data.Utilities;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChitalishteIskra.Core.Services
{
    public class ImageService:IImageService
    {
        private readonly Cloudinary cloudinary;

        public ImageService(IOptions<CloudinarySettings> options)
        {
            var settings = options.Value;

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret);

            cloudinary = new Cloudinary(account);
        }

        public async Task<string?> UploadImageAsync(IFormFile image, string fileName)
        {
            if (image == null || image.Length == 0)
            {
                return null;
            }

            await using var stream = image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            return uploadResult.SecureUrl?.ToString();
        }
    }
}
