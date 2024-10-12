using AutoFixture;
using AutoMapper;
using FakeItEasy;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Services;
using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;
using Homeverse.Infrastructure.Data;
using Homeverse.UnitTest.Mocks;

namespace Homeverse.UnitTest.Services;

public class MessageServiceTests
{
    private readonly Fixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMessageRepository _messageRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMessageService _sut;

    public MessageServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        var context = MockDbContext.CreateMockDbContext();
        _unitOfWork = new UnitOfWork(context);
        _mapper = A.Fake<IMapper>();
        _messageRepository = A.Fake<IMessageRepository>();
        _currentUserService = A.Fake<ICurrentUserService>();
        _sut = new MessageService(_unitOfWork, _mapper, _messageRepository, _currentUserService);
    }

    [Fact]
    public async Task GetMessageThreadAsync_WhenSuccessful_ShouldReturnMessages()
    {
        // Arrange
        var otherId = _fixture.Create<int>();
        var messages = _fixture.CreateMany<Message>(3);
        var response = _fixture.CreateMany<MessageResponse>(3);
        A.CallTo(() => _messageRepository.GetMessageThreadAsync(A<int>._, A<int>._)).Returns(messages);
        A.CallTo(() => _mapper.Map<IEnumerable<MessageResponse>>(A<IEnumerable<Message>>._)).Returns(response);

        // Act
        var actual = await _sut.GetMessageThreadAsync(otherId);

        // Assert
        A.CallTo(() => _messageRepository.GetMessageThreadAsync(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<MessageResponse>>(actual);
        Assert.Equal(messages.Count(), actual.Count());
    }

    [Fact]
    public async Task GetMessageByIdAsync_WhenSuccessful_ShouldReturnMessage()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var message = _fixture.Create<Message>();
        var response = _fixture.Create<MessageResponse>();
        A.CallTo(() => _messageRepository.GetMessageByIdAsync(A<int>._)).Returns(message);
        A.CallTo(() => _mapper.Map<MessageResponse>(A<Message>._)).Returns(response);

        // Act
        var actual = await _sut.GetMessageByIdAsync(id);

        // Assert
        A.CallTo(() => _messageRepository.GetMessageByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<MessageResponse>(actual);
    }

    [Fact]
    public async Task ReadMessageThreadAsync_WhenSuccessful_ShouldReadMessage()
    {
        // Arrange
        var otherId = _fixture.Create<int>();

        // Act
        await _sut.ReadMessageThreadAsync(otherId);

        // Assert
        A.CallTo(() => _messageRepository.ReadMessageThreadAsync(A<int>._, A<int>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task SendMessageAsync_WhenSuccessful_ShouldSendMessage()
    {
        // Arrange
        var request = _fixture.Create<MessageRequest>();
        var message = _fixture.Create<Message>();
        var response = _fixture.Create<MessageResponse>();
        A.CallTo(() => _mapper.Map<Message>(A<MessageRequest>._)).Returns(message);
        A.CallTo(() => _mapper.Map<MessageResponse>(A<Message>._)).Returns(response);

        // Act
        var actual = await _sut.SendMessageAsync(request);

        // Assert
        A.CallTo(() => _messageRepository.AddMessageAsync(A<Message>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<MessageResponse>(actual);
    }
}
