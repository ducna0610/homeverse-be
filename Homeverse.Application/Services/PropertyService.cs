using AutoMapper;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Interfaces;
using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Homeverse.Application.Services;

public interface IPropertyService
{
    Task<IEnumerable<PropertyResponse>> GetPropertiesAsync();
    Task<IEnumerable<PropertyResponse>> GetAllPropertiesAsync();
    Task<IEnumerable<PropertyDetailResponse>> GetAllPropertiesForUserAsync();
    Task<PropertyDetailResponse> GetPropertyByIdAsync(int id);
    Task<PropertyResponse> AddPropertyAsync(PropertyRequest request);
    Task<PropertyResponse> UpdatePropertyAsync(int id, PropertyRequest request);
    Task DeletePropertyAsync(int id);
    Task<PhotoResponse> AddPhotoAsync(IFormFile file, int propId);
    Task<PhotoResponse> SetPrimaryPhotoAsync(string photoPublicId);
    Task DeletePhotoAsync(string photoPublicId);
    Task<IEnumerable<PropertyResponse>> GetBookmarksAsync();
    Task AddBookmarkAsync(int propId);
    Task DeleteBookmarkAsync(int propId);
}

public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;

    public PropertyService
        (IUnitOfWork unitOfWork, IMapper mapper, IPropertyRepository propertyRepository, IFileStorageService fileStorageService, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _propertyRepository = propertyRepository;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<PropertyResponse>> GetPropertiesAsync()
    {
        var properties = await _propertyRepository.GetPropertiesAsync();

        return _mapper.Map<IEnumerable<PropertyResponse>>(properties);
    }

    public async Task<IEnumerable<PropertyResponse>> GetAllPropertiesAsync()
    {
        var properties = await _propertyRepository.GetAllPropertiesAsync();

        return _mapper.Map<IEnumerable<PropertyResponse>>(properties);
    }

    public async Task<IEnumerable<PropertyDetailResponse>> GetAllPropertiesForUserAsync()
    {
        int userId = _currentUserService.UserId;
        var properties = await _propertyRepository.GetAllPropertiesForUserAsync(userId);

        return _mapper.Map<IEnumerable<PropertyDetailResponse>>(properties);
    }

    public async Task<PropertyDetailResponse> GetPropertyByIdAsync(int id)
    {
        var property = await _propertyRepository.GetPropertyByIdAsync(id);

        return _mapper.Map<PropertyDetailResponse>(property);
    }

    public async Task<PropertyResponse> AddPropertyAsync(PropertyRequest request)
    {
        var property = _mapper.Map<Property>(request);
        property.PostedBy = _currentUserService.UserId;

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _propertyRepository.AddPropertyAsync(property);
        });

        foreach (var file in request.Images)
        {
            await AddPhotoAsync(file, property.Id);
        }

        return _mapper.Map<PropertyResponse>(property);
    }

    public async Task<PropertyResponse> UpdatePropertyAsync(int id, PropertyRequest request)
    {
        var oldProperty = await _propertyRepository.GetPropertyByIdAsync(id);

        var property = _mapper.Map<Property>(request);
        property.Id = oldProperty.Id;
        property.PostedBy = oldProperty.PostedBy;
        property.CreatedAt = oldProperty.CreatedAt;
        property.UpdatedAt = DateTimeOffset.Now;

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _propertyRepository.UpdatePropertyAsync(property);
        });

        var photos = await _propertyRepository.GetPhotosByPropertyIdAsync(property.Id);
        foreach (var photo in photos)
        {
            await DeletePhotoAsync(photo.PublicId);
        }
        foreach (var file in request.Images)
        {
            await AddPhotoAsync(file, property.Id);
        }

        return _mapper.Map<PropertyResponse>(property);
    }

    public async Task DeletePropertyAsync(int id)
    {
        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _propertyRepository.DeletePropertyAsync(id);
        });
    }

    public async Task<PhotoResponse> AddPhotoAsync(IFormFile file, int propId)
    {
        var property = await _propertyRepository.GetPropertyByIdAsync(propId);
        var result = await _fileStorageService.UploadAsync(file);
        var photo = new Photo()
        {
            ImageUrl = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            PropertyId = propId
        };

        if (property.Photos.FirstOrDefault() == null)
        {
            photo.IsPrimary = true;
        }

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _propertyRepository.AddPhotoAsync(photo);
        });

        return _mapper.Map<PhotoResponse>(photo);
    }

    public async Task<PhotoResponse> SetPrimaryPhotoAsync(string photoPublicId)
    {
        var photo = await _propertyRepository.SetPrimaryPhotoAsync(photoPublicId);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PhotoResponse>(photo);
    }

    public async Task DeletePhotoAsync(string photoPublicId)
    {
        await _fileStorageService.DeleteAsync(photoPublicId);
        await _propertyRepository.DeletePhotoAsync(photoPublicId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<PropertyResponse>> GetBookmarksAsync()
    {
        var properties = await _propertyRepository.GetBookmarksAsync(_currentUserService.UserId);

        return _mapper.Map<IEnumerable<PropertyResponse>>(properties);
    }

    public async Task AddBookmarkAsync(int propId)
    {
        var bookmark = new Bookmark() { UserId = _currentUserService.UserId, PropertyId = propId };
        await _propertyRepository.AddBookmarkAsync(bookmark);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteBookmarkAsync(int propId)
    {
        await _propertyRepository.DeleteBookmarkAsync(_currentUserService.UserId, propId);
        await _unitOfWork.SaveChangesAsync();
    }
}
