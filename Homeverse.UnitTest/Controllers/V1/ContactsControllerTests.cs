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

public class ContactsControllerTests
{
    private readonly Fixture _fixture;
    private readonly ILogger<ContactsController> _logger;
    private readonly IContactService _contactService;
    private readonly ICacheService _cacheService;
    private readonly ContactsController _sut;

    public ContactsControllerTests()
    {
        _fixture = new Fixture();
        _logger = A.Fake<ILogger<ContactsController>>();
        _contactService = A.Fake<IContactService>();
        _cacheService = A.Fake<ICacheService>();
        _sut = new ContactsController(_logger, _contactService, _cacheService);
    }

    [Fact]
    public async Task Get_WhenThereIsCacheData_ShouldReturnContactsWithStatusCode200OK()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<ContactResponse>(3).ToList();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).Returns(cacheData);

        // Act
        var actual = await _sut.Get();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<ContactResponse>>(actionResult.Value);
        Assert.Equal(cacheData.Count(), result.Count());
    }

    [Fact]
    public async Task Get_WhenThereAreContacts_ShouldReturnContactsWithStatusCode200OK()
    {
        // Arrange
        var cacheData = (IEnumerable<ContactResponse>)null;
        var response = _fixture.CreateMany<ContactResponse>(3).ToList();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).Returns(cacheData);
        A.CallTo(() => _contactService.GetContactsAsync()).Returns(response);

        // Act
        var actual = await _sut.Get();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _contactService.GetContactsAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.SetDataAsync<IEnumerable<ContactResponse>>("contacts", response)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        var result = Assert.IsAssignableFrom<IEnumerable<ContactResponse>>(actionResult.Value);
        Assert.Equal(response.Count(), result.Count());
    }

    [Fact]
    public async Task Get_WhenThereAreNoContactsFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var cacheData = (IEnumerable<ContactResponse>)null;
        var response = new List<ContactResponse>();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).Returns(cacheData);
        A.CallTo(() => _contactService.GetContactsAsync()).Returns(response);

        // Act
        var actual = await _sut.Get();

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _contactService.GetContactsAsync()).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task Get_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var cacheData = (IEnumerable<ContactResponse>)null;
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).Returns(cacheData);
        A.CallTo(() => _contactService.GetContactsAsync()).Throws<Exception>();

        // Act
        var actual = await _sut.Get() as StatusCodeResult;

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _contactService.GetContactsAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenThereIsContact_ShouldReturnContactWithStatusCode200OK()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<ContactResponse>(3).ToList();
        var id = _fixture.Create<int>();
        var response = _fixture.Create<ContactResponse>();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).Returns(cacheData);
        A.CallTo(() => _contactService.GetContactByIdAsync(A<int>._)).Returns(response);

        // Act
        var actual = await _sut.GetById(id);

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _contactService.GetContactByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<OkObjectResult>(actual);
        Assert.IsType<ContactResponse>(actionResult.Value);
    }

    [Fact]
    public async Task GetById_WhenThereIsNoContactFound_ShouldReturnStatusCode404NotFound()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<ContactResponse>(3).ToList();
        var id = _fixture.Create<int>();
        var response = new ContactResponse();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).Returns(cacheData);
        A.CallTo(() => _contactService.GetContactByIdAsync(A<int>._)).Returns(response);

        // Act
        var actual = await _sut.GetById(id);

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _contactService.GetContactByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsType<NotFoundResult>(actual);
    }

    [Fact]
    public async Task GetById_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var cacheData = _fixture.CreateMany<ContactResponse>(3).ToList();
        var id = _fixture.Create<int>();
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).Returns(cacheData);
        A.CallTo(() => _contactService.GetContactByIdAsync(A<int>._)).Throws<Exception>();

        // Act
        var actual = await _sut.GetById(id) as StatusCodeResult;

        // Assert
        A.CallTo(() => _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts")).MustHaveHappenedOnceExactly();
        A.CallTo(() => _contactService.GetContactByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
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
        var request = _fixture.Create<ContactRequest>();
        var response = _fixture.Create<ContactResponse>();
        A.CallTo(() => _contactService.AddContactAsync(A<ContactRequest>._)).Returns(response);

        // Act
        var actual = await _sut.Add(request);

        // Assert
        A.CallTo(() => _contactService.AddContactAsync(A<ContactRequest>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.RemoveDataAsync("contacts")).MustHaveHappenedOnceExactly();
        var actionResult = Assert.IsType<CreatedAtActionResult>(actual);
        Assert.IsType<ContactResponse>(actionResult.Value);
    }

    [Fact]
    public async Task Add_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var request = _fixture.Create<ContactRequest>();
        A.CallTo(() => _contactService.AddContactAsync(A<ContactRequest>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Add(request) as StatusCodeResult;

        // Assert
        A.CallTo(() => _contactService.AddContactAsync(A<ContactRequest>._)).MustHaveHappenedOnceExactly();
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
        A.CallTo(() => _contactService.DeleteContactAsync(A<int>._)).Returns(Task.CompletedTask);

        // Act
        var actual = await _sut.Delete(id);

        // Assert
        A.CallTo(() => _contactService.DeleteContactAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _cacheService.RemoveDataAsync("contacts")).MustHaveHappenedOnceExactly();
        Assert.IsType<NoContentResult>(actual);
    }

    [Fact]
    public async Task Delete_WhenThereIsUnhandledException_ShouldReturnStatusCode500InternalServerErrorAndLogAnException()
    {
        // Arrange
        var id = _fixture.Create<int>();
        A.CallTo(() => _contactService.DeleteContactAsync(A<int>._)).Throws<Exception>();

        // Act
        var actual = await _sut.Delete(id) as StatusCodeResult;

        // Assert
        A.CallTo(() => _contactService.DeleteContactAsync(A<int>._)).MustHaveHappenedOnceExactly();
        A.CallTo(_logger).Where(
                call => call.Method.Name == "Log"
                && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappened(1, Times.Exactly);
        Assert.Equal(StatusCodes.Status500InternalServerError, actual.StatusCode);
    }
}
