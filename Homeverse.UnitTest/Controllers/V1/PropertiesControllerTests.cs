using AutoFixture;
using FakeItEasy;
using Homeverse.API.Controllers.V1;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Interfaces;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Homeverse.UnitTest.Controllers.V1;

public class PropertiesControllerTests
{
    private readonly Fixture _fixture;
    private readonly ILogger<PropertiesController> _logger;
    private readonly IPropertyService _propertyService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;
    private readonly PropertiesController _sut;

    public PropertiesControllerTests()
    {
        _fixture = new Fixture();
        _fixture.Register<IFormFile>(() => null);
        _logger = A.Fake<ILogger<PropertiesController>>();
        _propertyService = A.Fake<IPropertyService>();
        _currentUserService = A.Fake<ICurrentUserService>();
        _cacheService = A.Fake<ICacheService>();
        _sut = new PropertiesController(_logger, _propertyService, _currentUserService, _cacheService);
    }

    [Fact]
    public async Task GetAll_WhenThereIsCacheData_ShouldReturnPropertiesWithStatusCode200OK()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<PropertyResponse>(3).ToList();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<PropertyResponse>>("properties")).Returns(cacheData);

        // Act
        var actual = await _sut.GetAll();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<PropertyResponse>>("properties")).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<PropertyResponse>>(actionResult.Value);
        Assert.Equal(cacheData.Count(), result.Count());
    }

    [Fact]
    public async Task GetAll_WhenThereAreProperties_ShouldReturnPropertiesWithStatusCode200OK()
    {
        // Arrange
        var cacheData = (IEnumerable<PropertyResponse>)null;
        var response = _fixture.CreateMany<PropertyResponse>(3).ToList();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<PropertyResponse>>("properties")).Returns(cacheData);
        A.CallTo(() => _propertyService.GetAllPropertiesAsync()).Returns(response);

        // Act
        var actual = await _sut.GetAll();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<PropertyResponse>>("properties")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _propertyService.GetAllPropertiesAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.SetDataAsync<IEnumerable<PropertyResponse>>("properties", response)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<PropertyResponse>>(actionResult.Value);
        Assert.Equal(response.Count(), result.Count());
    }

    [Fact]
    public async Task GetAll_WhenThereAreNoPropertiesFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var cacheData = (IEnumerable<PropertyResponse>)null;
        var response = new List<PropertyResponse>();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<PropertyResponse>>("properties")).Returns(cacheData);
        A.CallTo(() => _propertyService.GetAllPropertiesAsync()).Returns(response);

        // Act
        var actual = await _sut.GetAll();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<PropertyResponse>>("properties")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _propertyService.GetAllPropertiesAsync()).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task GetAll_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var cacheData = (IEnumerable<PropertyResponse>)null;
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<PropertyResponse>>("properties")).Returns(cacheData);
        A.CallTo(() => _propertyService.GetAllPropertiesAsync()).Throws<Exception>();

        // Act
        var actual = await _sut.GetAll() as StatusCodeResult;

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<PropertyResponse>>("properties")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _propertyService.GetAllPropertiesAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task GetActive_WhenThereAreProperties_ShouldReturnPropertiesWithStatusCode200OK()
    {
        // Arrange
        var response = _fixture.CreateMany<PropertyResponse>(3).ToList();
        A.CallTo(() => _propertyService.GetPropertiesAsync()).Returns(response);

        // Act
        var actual = await _sut.GetActive();

        // Assert
        A.CallTo(() => _propertyService.GetPropertiesAsync()).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<PropertyResponse>>(actionResult.Value);
        Assert.Equal(response.Count(), result.Count());
    }

    [Fact]
    public async Task GetActive_WhenThereAreNoPropertiesFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var response = new List<PropertyResponse>();
        A.CallTo(() => _propertyService.GetPropertiesAsync()).Returns(response);

        // Act
        var actual = await _sut.GetActive();

        // Assert
        A.CallTo(() => _propertyService.GetPropertiesAsync()).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task GetActive_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        A.CallTo(() => _propertyService.GetPropertiesAsync()).Throws<Exception>();

        // Act
        var actual = await _sut.GetActive() as StatusCodeResult;

        // Assert
        A.CallTo(() => _propertyService.GetPropertiesAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task GetForUser_WhenThereAreProperties_ShouldReturnPropertiesWithStatusCode200OK()
    {
        // Arrange
        var response = _fixture.CreateMany<PropertyDetailResponse>(3).ToList();
        A.CallTo(() => _propertyService.GetAllPropertiesForUserAsync()).Returns(response);

        // Act
        var actual = await _sut.GetForUser();

        // Assert
        A.CallTo(() => _propertyService.GetAllPropertiesForUserAsync()).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<PropertyDetailResponse>>(actionResult.Value);
        Assert.Equal(response.Count(), result.Count());
    }

    [Fact]
    public async Task GetForUser_WhenThereAreNoPropertiesFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var response = new List<PropertyDetailResponse>();
        A.CallTo(() => _propertyService.GetAllPropertiesForUserAsync()).Returns(response);

        // Act
        var actual = await _sut.GetForUser();

        // Assert
        A.CallTo(() => _propertyService.GetAllPropertiesForUserAsync()).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task GetForUser_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        A.CallTo(() => _propertyService.GetAllPropertiesForUserAsync()).Throws<Exception>();

        // Act
        var actual = await _sut.GetForUser() as StatusCodeResult;

        // Assert
        A.CallTo(() => _propertyService.GetAllPropertiesForUserAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenThereIsProperty_ShouldReturnPropertyWithStatusCode200OK()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var response = _fixture.Create<PropertyDetailResponse>();
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(response);

        // Act
        var actual = await _sut.GetById(id);

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<PropertyDetailResponse>(actionResult.Value);
    }

    [Fact]
    public async Task GetById_WhenThereIsNoPropertyFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var response = (PropertyDetailResponse)null;
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(response);

        // Act
        var actual = await _sut.GetById(id);

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task GetById_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var id = _fixture.Create<int>();
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Throws<Exception>();

        // Act
        var actual = await _sut.GetById(id) as StatusCodeResult;

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task Add_WhenSuccessful_ShouldReturnStatusCode201Created()
    {
        // Arrange
        var request = _fixture.Create<PropertyRequest>();
        var response = _fixture.Create<PropertyResponse>();
        A.CallTo(() => _propertyService.AddPropertyAsync(A<PropertyRequest>._)).Returns(response);

        // Act
        var actual = await _sut.Add(request);

        // Assert
        A.CallTo(() => _propertyService.AddPropertyAsync(A<PropertyRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.RemoveDataAsync("properties")).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<CreatedAtActionResult>(actual);
        Assert.IsType<PropertyResponse>(actionResult.Value);
    }

    [Fact]
    public async Task Add_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var request = _fixture.Create<PropertyRequest>();
        A.CallTo(() => _propertyService.AddPropertyAsync(A<PropertyRequest>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Add(request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _propertyService.AddPropertyAsync(A<PropertyRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task Update_WhenSuccessful_ShouldReturnCityWithStatusCode200OK()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var request = _fixture.Create<PropertyRequest>();
        var response = _fixture.Create<PropertyResponse>();
        A.CallTo(() => _propertyService.UpdatePropertyAsync(A<int>._, A<PropertyRequest>._)).Returns(response);

        // Act
        var actual = await _sut.Update(id, request);

        // Assert
        A.CallTo(() => _propertyService.UpdatePropertyAsync(A<int>._, A<PropertyRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.RemoveDataAsync("properties")).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<PropertyResponse>(actionResult.Value);
    }

    [Fact]
    public async Task Update_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var request = _fixture.Create<PropertyRequest>();
        A.CallTo(() => _propertyService.UpdatePropertyAsync(A<int>._, A<PropertyRequest>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Update(id, request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _propertyService.UpdatePropertyAsync(A<int>._, A<PropertyRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenSuccessful_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var id = _fixture.Create<int>();
        A.CallTo(() => _propertyService.DeletePropertyAsync(A<int>._)).Returns(Task.CompletedTask);

        // Act
        var actual = await _sut.Delete(id);

        // Assert
        A.CallTo(() => _propertyService.DeletePropertyAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.RemoveDataAsync("properties")).MustHaveHappenedOnceExactly();
        Assert.IsType<NoContentResult>(actual);
    }

    [Fact]
    public async Task Delete_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var id = _fixture.Create<int>();
        A.CallTo(() => _propertyService.DeletePropertyAsync(A<int>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Delete(id) as StatusCodeResult;

        // Assert
        A.CallTo(() => _propertyService.DeletePropertyAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task SetPrimaryPhoto_WhenSuccessful_ShouldReturnPhotoWithStatusCode200OK()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        var photoPublicId = _fixture.Create<string>();
        var property = _fixture.Create<PropertyDetailResponse>();
        property.PostedBy.Id = 0;
        property.Photos = new List<PhotoResponse>();
        property.Photos.Add(new PhotoResponse
        {
            PublicId = photoPublicId,
            IsPrimary = false,
        });
        var response = _fixture.Create<PhotoResponse>();
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(property);
        A.CallTo(() => _propertyService.SetPrimaryPhotoAsync(A<string>._)).Returns(response);

        // Act
        var actual = await _sut.SetPrimaryPhoto(propId, photoPublicId);

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _propertyService.SetPrimaryPhotoAsync(A<string>._)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<PhotoResponse>(actionResult.Value);
    }

    [Fact]
    public async Task SetPrimaryPhoto_WhenNotAuthorised_ShouldReturnPhotoWithStatusCode200OK()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        var photoPublicId = _fixture.Create<string>();
        var property = _fixture.Create<PropertyDetailResponse>();
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(property);

        // Act
        var actual = await _sut.SetPrimaryPhoto(propId, photoPublicId);

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<BadRequestObjectResult>(actual);
    }

    [Fact]
    public async Task SetPrimaryPhoto_WhenThereIsNoPropertyFound_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        var photoPublicId = _fixture.Create<string>();
        var property = new PropertyDetailResponse();
        property.PostedBy = _fixture.Create<UserResponse>();
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(property);

        // Act
        var actual = await _sut.SetPrimaryPhoto(propId, photoPublicId);

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<BadRequestObjectResult>(actual);
    }

    [Fact]
    public async Task SetPrimaryPhoto_WhenThereIsNoPhotoFound_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        var photoPublicId = _fixture.Create<string>();
        var property = _fixture.Create<PropertyDetailResponse>();
        property.PostedBy.Id = 0;
        var response = _fixture.Create<PhotoResponse>();
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(property);

        // Act
        var actual = await _sut.SetPrimaryPhoto(propId, photoPublicId);

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<BadRequestObjectResult>(actual);
    }

    [Fact]
    public async Task SetPrimaryPhoto_WhenPhotoAlreadyPrimary_ShouldReturnPhotoWithStatusCode200OK()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        var photoPublicId = _fixture.Create<string>();
        var property = _fixture.Create<PropertyDetailResponse>();
        property.PostedBy.Id = 0;
        property.Photos = new List<PhotoResponse>();
        property.Photos.Add(new PhotoResponse
        {
            PublicId = photoPublicId,
            IsPrimary = true,
        });
        var response = _fixture.Create<PhotoResponse>();
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(property);

        // Act
        var actual = await _sut.SetPrimaryPhoto(propId, photoPublicId);

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<BadRequestObjectResult>(actual);
    }

    [Fact]
    public async Task SetPrimaryPhoto_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        var photoPublicId = _fixture.Create<string>();
        var property = _fixture.Create<PropertyDetailResponse>();
        property.PostedBy.Id = 0;
        property.Photos = new List<PhotoResponse>();
        property.Photos.Add(new PhotoResponse
        {
            PublicId = photoPublicId,
            IsPrimary = false,
        });
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(property);
        A.CallTo(() => _propertyService.SetPrimaryPhotoAsync(A<string>._)).Throws<Exception>();

        // Act
        var actual = await _sut.SetPrimaryPhoto(propId, photoPublicId) as StatusCodeResult;

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _propertyService.SetPrimaryPhotoAsync(A<string>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task GetBookmarks_WhenThereAreBookmarks_ShouldReturnBookmarksWithStatusCode200OK()
    {
        // Arrange
        var response = _fixture.CreateMany<PropertyResponse>(3).ToList();
        A.CallTo(() => _propertyService.GetBookmarksAsync()).Returns(response);

        // Act
        var actual = await _sut.GetBookmarks();

        // Assert
        A.CallTo(() => _propertyService.GetBookmarksAsync()).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<PropertyResponse>>(actionResult.Value);
        Assert.Equal(response.Count(), result.Count());
    }

    [Fact]
    public async Task GetBookmarks_WhenThereAreNoBookmarksFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var response = new List<PropertyResponse>();
        A.CallTo(() => _propertyService.GetBookmarksAsync()).Returns(response);

        // Act
        var actual = await _sut.GetBookmarks();

        // Assert
        A.CallTo(() => _propertyService.GetBookmarksAsync()).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task GetBookmarks_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        A.CallTo(() => _propertyService.GetBookmarksAsync()).Throws<Exception>();

        // Act
        var actual = await _sut.GetBookmarks() as StatusCodeResult;

        // Assert
        A.CallTo(() => _propertyService.GetBookmarksAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task AddBookmark_WhenSuccessful_ShouldReturnBookmarksWithStatusCode200OK()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        var property = _fixture.Create<PropertyDetailResponse>();
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(property);

        // Act
        var actual = await _sut.AddBookmark(propId);

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _propertyService.GetBookmarksAsync()).MustHaveHappenedTwiceExactly();
        A.CallTo(() => _propertyService.AddBookmarkAsync(A<int>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task AddBookmark_WhenPropertyNotFound_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        var property = new PropertyDetailResponse();
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(property);

        // Act
        var actual = await _sut.AddBookmark(propId);

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<BadRequestObjectResult>(actual);
    }

    [Fact]
    public async Task AddBookmark_WhenAlreadyBookmark_ShouldReturnStatusCode400BadRequest()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        var property = new PropertyDetailResponse();
        var bookmarks = new List<PropertyResponse>();
        bookmarks.Add(property);
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(property);
        A.CallTo(() => _propertyService.GetBookmarksAsync()).Returns(bookmarks);

        // Act
        var actual = await _sut.AddBookmark(propId);

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<BadRequestObjectResult>(actual);
    }

    [Fact]
    public async Task AddBookmark_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        var property = _fixture.Create<PropertyDetailResponse>();
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).Returns(property);
        A.CallTo(() => _propertyService.AddBookmarkAsync(A<int>._)).Throws<Exception>();

        // Act
        var actual = await _sut.AddBookmark(propId) as StatusCodeResult;

        // Assert
        A.CallTo(() => _propertyService.GetPropertyByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _propertyService.GetBookmarksAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(() => _propertyService.AddBookmarkAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task DeleteBookmark_WhenSuccessful_ShouldReturnPropertiesWithStatusCode200OK()
    {
        // Arrange
        var propId = _fixture.Create<int>();

        // Act
        var actual = await _sut.DeleteBookmark(propId);

        // Assert
        A.CallTo(() => _propertyService.DeleteBookmarkAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _propertyService.GetBookmarksAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DeleteBookmark_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var propId = _fixture.Create<int>();
        A.CallTo(() => _propertyService.DeleteBookmarkAsync(A<int>._)).Throws<Exception>();

        // Act
        var actual = await _sut.DeleteBookmark(propId) as StatusCodeResult;

        // Assert
        A.CallTo(() => _propertyService.DeleteBookmarkAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }
}
