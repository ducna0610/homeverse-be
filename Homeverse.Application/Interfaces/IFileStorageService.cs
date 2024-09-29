using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Homeverse.Application.Interfaces;

public interface IFileStorageService
{
    Task<ImageUploadResult> UploadAsync(IFormFile image);
    Task<DeletionResult> DeleteAsync(string publicId);
}
