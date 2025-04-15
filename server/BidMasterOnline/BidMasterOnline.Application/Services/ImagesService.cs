using BidMasterOnline.Application.Enums;
using BidMasterOnline.Application.ServiceContracts;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BidMasterOnline.Application.Services
{
    public class ImagesService : IImagesService
    {
        private readonly Cloudinary _cloudinary;

        public ImagesService(IConfiguration configuration)
        {
            var account = new Account(
                    configuration["CloudinarySettings:CloudName"]!,
                    configuration["CloudinarySettings:APIKey"]!,
                    configuration["CloudinarySettings:APISecret"]!
                );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResult> AddImageAsync(IFormFile image, ImageType type)
        {
            var uploadResult = new ImageUploadResult();

            if (image.Length > 0)
            {
                using var stream = image.OpenReadStream();

                int height = 0, width = 0;

                switch(type)
                {
                    case ImageType.ImageForAuction:
                        height = 720;
                        width = 540;
                        break;
                    case ImageType.ImageForUser:
                        height = 500;
                        width = 500;
                        break;
                }

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(image.FileName, stream),
                    Transformation = new Transformation()
                        .Height(height)
                        .Width(width)
                        .Crop("fill")
                        .Gravity("face")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
