using AutoFixture;
using AutoMapper;
using CloudinaryDotNet.Actions;
using FakeItEasy;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Interfaces;
using Homeverse.Application.Services;
using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Homeverse.UnitTest.Mocks;
using Microsoft.AspNetCore.Http;

namespace Homeverse.UnitTest.Services;

public class PropertyServiceTests
{
    private readonly Fixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPropertyService _sut;

    public PropertyServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _fixture.Register<IFormFile>(() => null);
        var context = MockDbContext.CreateMockDbContext();
        _unitOfWork = new UnitOfWork(context);
        _mapper = A.Fake<IMapper>();
        _propertyRepository = A.Fake<IPropertyRepository>();
        _fileStorageService = A.Fake<IFileStorageService>();
        _currentUserService = A.Fake<ICurrentUserService>();
        _sut = new PropertyService(_unitOfWork, _mapper, _propertyRepository, _fileStorageService, _currentUserService);
    }

    [Fact]
    public async Task GetPropertiesAsync_WhenSuccessful_ShouldReturnProperties()
    {
        // Arrange
        var properties = _fixture.CreateMany<Property>(3).ToList();
        var response = _fixture.CreateMany<PropertyResponse>(3).ToList();
        A.CallTo(() => _propertyRepository.GetPropertiesAsync()).Returns(properties);
        A.CallTo(() => _mapper.Map<IEnumerable<PropertyResponse>>(A<IEnumerable<Property>>._)).Returns(response);

        // Act
        var actual = await _sut.GetPropertiesAsync();

        // Assert
        A.CallTo(() => _propertyRepository.GetPropertiesAsync()).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<PropertyResponse>>(actual);
        Assert.Equal(properties.Count(), actual.Count());
    }

    [Fact]
    public async Task GetAllPropertiesAsync_WhenSuccessful_ShouldReturnProperties()
    {
        // Arrange
        var properties = _fixture.CreateMany<Property>(3).ToList();
        var response = _fixture.CreateMany<PropertyDetailResponse>(3).ToList();
        A.CallTo(() => _propertyRepository.GetAllPropertiesAsync()).Returns(properties);
        A.CallTo(() => _mapper.Map<IEnumerable<PropertyDetailResponse>>(A<IEnumerable<Property>>._)).Returns(response);

        // Act
        var actual = await _sut.GetAllPropertiesAsync();

        // Assert
        A.CallTo(() => _propertyRepository.GetAllPropertiesAsync()).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<PropertyDetailResponse>>(actual);
        Assert.Equal(properties.Count(), actual.Count());
    }

    [Fact]
    public async Task GetAllPropertiesForUserAsync_WhenSuccessful_ShouldReturnProperties()
    {
        // Arrange
        var properties = _fixture.CreateMany<Property>(3).ToList();
        var response = _fixture.CreateMany<PropertyDetailResponse>(3).ToList();
        A.CallTo(() => _propertyRepository.GetAllPropertiesForUserAsync(A<int>._)).Returns(properties);
        A.CallTo(() => _mapper.Map<IEnumerable<PropertyDetailResponse>>(A<IEnumerable<Property>>._)).Returns(response);

        // Act
        var actual = await _sut.GetAllPropertiesForUserAsync();

        // Assert
        A.CallTo(() => _propertyRepository.GetAllPropertiesForUserAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<PropertyDetailResponse>>(actual);
        Assert.Equal(properties.Count(), actual.Count());
    }

    [Fact]
    public async Task GetPropertyByIdAsync_WhenSuccessful_ShouldReturnProperty()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var property = _fixture.Create<Property>();
        var response = _fixture.Create<PropertyDetailResponse>();
        A.CallTo(() => _propertyRepository.GetPropertyByIdAsync(A<int>._)).Returns(property);
        A.CallTo(() => _mapper.Map<PropertyDetailResponse>(A<Property>._)).Returns(response);

        // Act
        var actual = await _sut.GetPropertyByIdAsync(id);

        // Assert
        A.CallTo(() => _propertyRepository.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<PropertyDetailResponse>(actual);
    }

    [Fact]
    public async Task AddPropertyAsync_WhenSuccessful_ShouldAddAndReturnProperty()
    {
        // Arrange
        var request = _fixture.Create<PropertyRequest>();
        var property = _fixture.Create<Property>();
        var response = _fixture.Create<PropertyResponse>();
        A.CallTo(() => _mapper.Map<Property>(A<PropertyRequest>._)).Returns(property);
        A.CallTo(() => _mapper.Map<PropertyResponse>(A<Property>._)).Returns(response);

        // Act
        var actual = await _sut.AddPropertyAsync(request);

        // Assert
        A.CallTo(() => _propertyRepository.AddPropertyAsync(A<Property>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<PropertyResponse>(actual);
    }

    [Fact]
    public async Task UpdatePropertyAsync_WhenSuccessful_ShouldUpdateAndReturnProperty()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var request = _fixture.Create<PropertyRequest>();
        var property = _fixture.Create<Property>();
        var response = _fixture.Create<PropertyResponse>();
        A.CallTo(() => _mapper.Map<Property>(A<PropertyResponse>._)).Returns(property);
        A.CallTo(() => _mapper.Map<PropertyResponse>(A<Property>._)).Returns(response);

        // Act
        var actual = await _sut.UpdatePropertyAsync(id, request);

        // Assert
        A.CallTo(() => _propertyRepository.UpdatePropertyAsync(A<Property>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<PropertyResponse>(actual);
    }

    [Fact]
    public async Task DeletePropertyAsync_WhenSuccessful_ShouldDeleteProperty()
    {
        // Arrange
        var id = _fixture.Create<int>();

        // Act
        await _sut.DeletePropertyAsync(id);

        // Assert
        A.CallTo(() => _propertyRepository.DeletePropertyAsync(A<int>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task AddPhotoAsync_WhenSuccessful_ShouldAddAndReturnPhoto()
    {
        // Arrange
        var file = _fixture.Create<IFormFile>();
        var propId = _fixture.Create<int>();
        var property = _fixture.Create<Property>();
        var result = _fixture.Create<ImageUploadResult>();
        A.CallTo(() => _propertyRepository.GetPropertyByIdAsync(A<int>._)).Returns(property);
        A.CallTo(() => _fileStorageService.UploadAsync(A<IFormFile>._)).Returns(result);

        // Act
        var actual = await _sut.AddPhotoAsync(file, propId);

        // Assert
        A.CallTo(() => _propertyRepository.AddPhotoAsync(A<Photo>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<PhotoResponse>(actual);
    }

    [Fact]
    public async Task SetPrimaryPhotoAsync_WhenSuccessful_ShouldUpdateAndReturnPhoto()
    {
        // Arrange
        var photoPublicId = _fixture.Create<string>();

        // Act
        var actual = await _sut.SetPrimaryPhotoAsync(photoPublicId);

        // Assert
        A.CallTo(() => _propertyRepository.SetPrimaryPhotoAsync(A<string>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<PhotoResponse>(actual);
    }

    [Fact]
    public async Task DeletePhotoAsync_WhenSuccessful_ShouldDeletePhoto()
    {
        // Arrange
        var photoPublicId = _fixture.Create<string>();

        // Act
        await _sut.DeletePhotoAsync(photoPublicId);

        // Assert
        A.CallTo(() => _fileStorageService.DeleteAsync(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _propertyRepository.DeletePhotoAsync(A<string>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetBookmarksAsync_WhenSuccessful_ShouldReturnBookmarks()
    {
        // Arrange
        var properties = _fixture.CreateMany<Property>(3).ToList();
        var response = _fixture.CreateMany<PropertyResponse>(3).ToList();
        A.CallTo(() => _propertyRepository.GetBookmarksAsync(A<int>._)).Returns(properties);
        A.CallTo(() => _mapper.Map<IEnumerable<PropertyResponse>>(A<IEnumerable<Property>>._)).Returns(response);

        // Act
        var actual = await _sut.GetBookmarksAsync();

        // Assert
        A.CallTo(() => _propertyRepository.GetBookmarksAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<PropertyResponse>>(actual);
        Assert.Equal(properties.Count(), actual.Count());
    }

    [Fact]
    public async Task AddBookmarkAsync_WhenSuccessful_ShouldAddBookmark()
    {
        // Arrange
        var propId = _fixture.Create<int>();

        // Act
        await _sut.AddBookmarkAsync(propId);

        // Assert
        A.CallTo(() => _propertyRepository.AddBookmarkAsync(A<Bookmark>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DeleteBookmarkAsync_WhenSuccessful_ShouldDeleteBookmark()
    {
        // Arrange
        var propId = _fixture.Create<int>();

        // Act
        await _sut.DeleteBookmarkAsync(propId);

        // Assert
        A.CallTo(() => _propertyRepository.DeleteBookmarkAsync(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
    }
}
