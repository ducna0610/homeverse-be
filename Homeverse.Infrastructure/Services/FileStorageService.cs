using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Homeverse.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Homeverse.Infrastructure.Services;

public class CloudinarySettings
{
    public string CloudName { get; set; } = null!;

    public string ApiKey { get; set; } = null!;

    public string ApiSecret { get; set; } = null!;
}

public class FileStorageService : IFileStorageService
{
    private readonly Cloudinary cloudinary;

    public FileStorageService(IOptions<CloudinarySettings> options)
    {
        Account account = new Account(options.Value.CloudName, options.Value.ApiKey, options.Value.ApiSecret);
        cloudinary = new Cloudinary(account);
    }

    public async Task<ImageUploadResult> UploadAsync(IFormFile image)
    {
        var uploadResult = new ImageUploadResult();
        if (image.Length > 0)
        {
            using var stream = image.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                Folder = "homeverse/",
                File = new FileDescription(image.FileName, stream),
                Transformation = new Transformation()
                    .Height(500).Width(800)
            };
            uploadResult = await cloudinary.UploadAsync(uploadParams);
        }
        return uploadResult;
    }

    public async Task<DeletionResult> DeleteAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        var result = await cloudinary.DestroyAsync(deleteParams);

        return result;
    }
}
