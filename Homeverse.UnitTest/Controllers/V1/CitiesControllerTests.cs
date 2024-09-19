using AutoFixture;
using FakeItEasy;
using Homeverse.API.Controllers.V1;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Homeverse.UnitTest.Controllers.V1;

public class CitiesControllerTests
{
    private readonly Fixture _fixture;
    private readonly ILogger<CitiesController> _logger;
    private readonly ICityService _cityService;
    private readonly CitiesController _sut;

    public CitiesControllerTests()
    {
        _fixture = new Fixture();
        _logger = A.Fake<ILogger<CitiesController>>();
        _cityService = A.Fake<ICityService>();
        _sut = new CitiesController(_logger, _cityService);
    }

    [Fact]
    public async Task Get_WhenThereAreCities_ShouldReturnCitiesWithStatusCode200OK()
    {
        // Arrange
        var response = _fixture.CreateMany<CityResponse>(3).ToList();
        A.CallTo(() => _cityService.GetCitiesAsync()).Returns(response);

        // Act
        var actual = await _sut.Get();

        // Assert
        A.CallTo(() => _cityService.GetCitiesAsync()).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<CityResponse>>(actionResult.Value);
        Assert.Equal(response.Count(), result.Count());
    }

    [Fact]
    public async Task Get_WhenThereAreNoCitiesFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var response = new List<CityResponse>();
        A.CallTo(() => _cityService.GetCitiesAsync()).Returns(response);

        // Act
        var actual = await _sut.Get() as StatusCodeResult;

        // Assert
        A.CallTo(() => _cityService.GetCitiesAsync()).MustHaveHappenedOnceExactly();
        Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
    }

    [Fact]
    [Trait("HttpVerb", "GET")]
    public async Task Get_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        A.CallTo(() => _cityService.GetCitiesAsync()).Throws<Exception>();

        // Act
        var actual = await _sut.Get() as StatusCodeResult;

        // Assert
        A.CallTo(() => _cityService.GetCitiesAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenThereIsCity_ShouldReturnCityWithStatusCode200OK()
    {
        // Arrange
        var request = _fixture.Create<int>();
        var response = _fixture.Create<CityResponse>();
        A.CallTo(() => _cityService.GetCityByIdAsync(A<int>._)).Returns(response);

        // Act
        var actual = await _sut.GetById(request);

        // Assert
        A.CallTo(() => _cityService.GetCityByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<CityResponse>(actionResult.Value);
    }

    [Fact]
    public async Task GetById_WhenThereAreNoCityFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var request = _fixture.Create<int>();
        var response = new CityResponse();
        A.CallTo(() => _cityService.GetCityByIdAsync(A<int>._)).Returns(response);

        // Act
        var actual = await _sut.GetById(request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _cityService.GetCityByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var id = _fixture.Create<int>();
        A.CallTo(() => _cityService.GetCityByIdAsync(A<int>._)).Throws<Exception>();

        // Act
        var actual = await _sut.GetById(id) as StatusCodeResult;

        // Assert
        A.CallTo(() => _cityService.GetCityByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task Add_WhenValid_ShouldReturnStatusCode201Created()
    {
        // Arrange
        var request = _fixture.Create<CityRequest>();
        var response = _fixture.Create<CityResponse>();
        A.CallTo(() => _cityService.AddCityAsync(A<CityRequest>._)).Returns(response);

        // Act
        var actual = await _sut.Add(request);

        // Assert
        A.CallTo(() => _cityService.AddCityAsync(A<CityRequest>._)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<CreatedAtActionResult>(actual);
        Assert.IsType<CityResponse>(actionResult.Value);
    }

    [Fact]
    public async Task Create_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var request = _fixture.Create<CityRequest>();
        A.CallTo(() => _cityService.AddCityAsync(A<CityRequest>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Add(request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _cityService.AddCityAsync(A<CityRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task Update_WhenValid_ShouldReturnClinicStatusCode200OK()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CityRequest>();
        var response = _fixture.Create<CityResponse>();
        A.CallTo(() => _cityService.UpdateCityAsync(A<int>._, A<CityRequest>._)).Returns(response);

        // Act
        var actual = await _sut.Update(id, request);

        // Assert
        A.CallTo(() => _cityService.UpdateCityAsync(A<int>._, A<CityRequest>._)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<CityResponse>(actionResult.Value);
    }

    [Fact]
    public async Task Update_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CityRequest>();
        A.CallTo(() => _cityService.UpdateCityAsync(A<int>._, A<CityRequest>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Update(id, request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _cityService.UpdateCityAsync(A<int>._, A<CityRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenValid_ShouldReturnStatusCode204NoContent()
    {
        // Arrange
        var id = _fixture.Create<int>();
        A.CallTo(() => _cityService.DeleteCityAsync(A<int>._)).Returns(Task.CompletedTask);

        // Act
        var actual = await _sut.Delete(id);

        // Assert
        A.CallTo(() => _cityService.DeleteCityAsync(A<int>._)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<NoContentResult>(actual);
    }

    [Fact]
    public async Task Delete_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var id = _fixture.Create<int>();
        A.CallTo(() => _cityService.DeleteCityAsync(A<int>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Delete(id) as StatusCodeResult;

        // Assert
        A.CallTo(() => _cityService.DeleteCityAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }
}
