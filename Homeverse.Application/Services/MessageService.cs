using AutoMapper;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;

namespace Homeverse.Application.Services;

public interface IMessageService
{
    Task<IEnumerable<MessageResponse>> GetMessageThreadAsync(int otherId);
    Task<MessageResponse> GetMessageByIdAsync(int id);
    Task ReadMessageThreadAsync(int otherId);
    Task<MessageResponse> SendMessageAsync(MessageRequest request);
}

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMessageRepository _messageRepository;
    private readonly ICurrentUserService _currentUserService;

    public MessageService
    (
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IMessageRepository messageRepository, 
        ICurrentUserService currentUserService
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _messageRepository = messageRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<MessageResponse>> GetMessageThreadAsync(int otherId)
    {
        var messages = await _messageRepository.GetMessageThreadAsync(_currentUserService.UserId, otherId);

        return _mapper.Map<IEnumerable<MessageResponse>>(messages);
    }

    public async Task<MessageResponse> GetMessageByIdAsync(int id)
    {
        var message = await _messageRepository.GetMessageByIdAsync(id);

        return _mapper.Map<MessageResponse>(message);
    }

    public async Task ReadMessageThreadAsync(int otherId)
    {
        await _messageRepository.ReadMessageThreadAsync(_currentUserService.UserId, otherId);
    }

    public async Task<MessageResponse> SendMessageAsync(MessageRequest request)
    {
        var message = _mapper.Map<Message>(request);
        message.SenderId = _currentUserService.UserId;

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _messageRepository.AddMessageAsync(message);
        });

        return await GetMessageByIdAsync(message.Id);
    }
}
