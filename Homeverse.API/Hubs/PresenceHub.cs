using Homeverse.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Homeverse.API.Hubs;

public class PresenceHub : Hub
{
    private readonly IUserService _userService;

    public PresenceHub
    (
        IUserService userService
    )
    {
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        await _userService.AddConnection(Context.ConnectionId);

        var friends = await _userService.GetFriends();
        await Clients.Caller.SendAsync("GetFriends", friends);

        await UpdateToFriends();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        await _userService.DeleteConnection(Context.ConnectionId);
        await UpdateToFriends();

        await base.OnDisconnectedAsync(ex);
    }

    private async Task UpdateToFriends()
    {
        var friendIds = await _userService.GetFriendIds();

        foreach (var friendId in friendIds)
        {
            var updateFriend = await _userService.GetFriend(friendId);
            var friendConnectionIds = await _userService.GetConnectionIdsByUserId(friendId);
            await Clients.Clients(friendConnectionIds).SendAsync("UpdateFriend", updateFriend);
        }
    }
}