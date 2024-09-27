using AutoFixture;
using FakeItEasy;
using Homeverse.API.Controllers.V1;
using Homeverse.Application.Interfaces;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Homeverse.UnitTest.Controllers.V1;

public class EnumsControllerTests
{
    private readonly Fixture _fixture;
    private readonly ILogger<EnumsController> _logger;
    private readonly IEnumService _enumService;
    private readonly ICacheService _cacheService;
    private readonly EnumsController _sut;

    public EnumsControllerTests()
    {
        _fixture = new Fixture();
        _logger = A.Fake<ILogger<EnumsController>>();
        _enumService = A.Fake<IEnumService>();
        _cacheService = A.Fake<ICacheService>();
        _sut = new EnumsController(_logger, _enumService, _cacheService);
    }

    [Fact]
    public async Task GetCategoryEnum_WhenThereIsCacheData_ShouldReturnCategoriesWithStatusCode200OK()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<KeyValuePair<int, string>>(3).ToList();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("categories")).Returns(cacheData);

        // Act
        var actual = await _sut.GetCategoryEnum();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("categories")).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<int, string>>>(actionResult.Value);
        Assert.Equal(cacheData.Count(), result.Count());
    }

    [Fact]
    public async Task GetCategoryEnum_WhenThereAreCategories_ShouldReturnCategoriesWithStatusCode200OK()
    {
        // Arrange
        var cacheData = (IEnumerable<KeyValuePair<int, string>>)null;
        var response = _fixture.CreateMany<KeyValuePair<int, string>>(3).ToList();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("categories")).Returns(cacheData);
        A.CallTo(() => _enumService.GetCaegoryEnum()).Returns(response);

        // Act
        var actual = await _sut.GetCategoryEnum();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("categories")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _enumService.GetCaegoryEnum()).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.SetDataAsync<IEnumerable<KeyValuePair<int, string>>>("categories", response)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<int, string>>>(actionResult.Value);
        Assert.Equal(response.Count(), result.Count());
    }

    [Fact]
    public async Task GetCategoryEnum_WhenThereAreNoCategoriesFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var cacheData = (IEnumerable<KeyValuePair<int, string>>)null;
        var response = new List<KeyValuePair<int, string>>();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("categories")).Returns(cacheData);
        A.CallTo(() => _enumService.GetCaegoryEnum()).Returns(response);

        // Act
        var actual = await _sut.GetCategoryEnum();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("categories")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _enumService.GetCaegoryEnum()).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task GetCategoryEnum_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var cacheData = (IEnumerable<KeyValuePair<int, string>>)null;
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("categories")).Returns(cacheData);
        A.CallTo(() => _enumService.GetCaegoryEnum()).Throws<Exception>();

        // Act
        var actual = await _sut.GetCategoryEnum() as StatusCodeResult;

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("categories")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _enumService.GetCaegoryEnum()).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task GetFurnishEnum_WhenThereIsCacheData_ShouldReturnFurnishesWithStatusCode200OK()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<KeyValuePair<int, string>>(3).ToList();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("furnishes")).Returns(cacheData);

        // Act
        var actual = await _sut.GetFurnishEnum();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("furnishes")).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<int, string>>>(actionResult.Value);
        Assert.Equal(cacheData.Count(), result.Count());
    }

    [Fact]
    public async Task GetFurnishEnum_WhenThereAreFurnishes_ShouldReturnFurnishesWithStatusCode200OK()
    {
        // Arrange
        var cacheData = (IEnumerable<KeyValuePair<int, string>>)null;
        var response = _fixture.CreateMany<KeyValuePair<int, string>>(3).ToList();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("furnishes")).Returns(cacheData);
        A.CallTo(() => _enumService.GetFurnishEnum()).Returns(response);

        // Act
        var actual = await _sut.GetFurnishEnum();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("furnishes")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _enumService.GetFurnishEnum()).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.SetDataAsync<IEnumerable<KeyValuePair<int, string>>>("furnishes", response)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<KeyValuePair<int, string>>>(actionResult.Value);
        Assert.Equal(response.Count(), result.Count());
    }

    [Fact]
    public async Task GetFurnishEnum_WhenThereAreNoFurnishesFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var cacheData = (IEnumerable<KeyValuePair<int, string>>)null;
        var response = new List<KeyValuePair<int, string>>();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("furnishes")).Returns(cacheData);
        A.CallTo(() => _enumService.GetFurnishEnum()).Returns(response);

        // Act
        var actual = await _sut.GetFurnishEnum();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("furnishes")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _enumService.GetFurnishEnum()).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task GetFurnishEnum_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var cacheData = (IEnumerable<KeyValuePair<int, string>>)null;
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("furnishes")).Returns(cacheData);
        A.CallTo(() => _enumService.GetFurnishEnum()).Throws<Exception>();

        // Act
        var actual = await _sut.GetFurnishEnum() as StatusCodeResult;

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("furnishes")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _enumService.GetFurnishEnum()).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }
}
