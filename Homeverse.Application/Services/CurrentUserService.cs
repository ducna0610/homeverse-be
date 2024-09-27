using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Homeverse.Application.Services;

public interface ICurrentUserService
{
    int UserId { get; }
    string IpAddress { get; }
    List<KeyValuePair<string, string>> Claims { get; set; }
}

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        UserId = int.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1");
        Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
        IpAddress = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public int UserId { get; }
    public string IpAddress { get; }
    public List<KeyValuePair<string, string>> Claims { get; set; }
}
