using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Homeverse.API.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHubContext<PresenceHub> _presenceHub;

    public ChatHub
    (
        IMessageService messageService, 
        IUserService userService,
        IHubContext<PresenceHub> presenceHub,
        ICurrentUserService currentUserService
    )
    {
        _messageService = messageService;
        _userService = userService;
        _presenceHub = presenceHub;
        _currentUserService = currentUserService;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUserId = httpContext?.Request.Query["otherId"];
        string groupName = GetGroupName(_currentUserService.UserId, int.Parse(otherUserId));
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await _messageService.GetMessageThreadAsync(int.Parse(otherUserId));
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        await ReadMessage(int.Parse(otherUserId));
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        await base.OnDisconnectedAsync(ex);
    }

    public async Task SendMessage(MessageRequest request)
    {
        if (request.ReceiverId == _currentUserService.UserId)
        {
            throw new HubException("You cannot send messages to yourself");
        }
        var receiverConnectionIds = await _userService.GetConnectionIdsByUserId(request.ReceiverId);
        var message = await _messageService.SendMessageAsync(request);

        await UpdateToFriend(message.Receiver.Id);
        await _presenceHub.Clients.Clients(receiverConnectionIds).SendAsync("NewMessageReceived", message);
        string groupName = GetGroupName(message.Sender.Id, message.Receiver.Id);
        await Clients.Group(groupName).SendAsync("NewMessage", message);
    }

    public async Task ReadMessage(int otherId)
    {
        await _messageService.ReadMessageThreadAsync(otherId);
        if (otherId != _currentUserService.UserId)
        {
            await UpdateToFriend(otherId);
        }
        string groupName = GetGroupName(_currentUserService.UserId, otherId);
        await Clients.Group(groupName).SendAsync("ReadedMessage", _currentUserService.UserId);
    }

    private string GetGroupName(int currentId, int otherId)
    {
        return currentId < otherId ? $"{currentId}-{otherId}" : $"{otherId}-{currentId}";
    }

    public async Task UpdateToFriend(int friendId)
    {
        var updateFriend = await _userService.GetFriend(friendId);
        var friendConnectionIds = await _userService.GetConnectionIdsByUserId(friendId);
        await _presenceHub.Clients.Clients(friendConnectionIds).SendAsync("UpdateFriend", updateFriend);
    }
}
