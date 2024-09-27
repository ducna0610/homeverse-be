using AutoMapper;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Domain.Entities;

namespace Homeverse.Application.Mappings;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CityRequest, City>();
        CreateMap<City, CityResponse>();

        CreateMap<ContactRequest, Contact>();
        CreateMap<Contact, ContactResponse>();

        CreateMap<RegisterRequest, User>()
            .ForMember(x => x.Name, y => y.MapFrom(z => z.UserName))
            .ForMember(x => x.PasswordHash, y => y.MapFrom(z => Convert.FromBase64String("")))
            .ForMember(x => x.PasswordSalt, y => y.MapFrom(z => Convert.FromBase64String("")));
        CreateMap<User, UserResponse>();
        CreateMap<User, FriendResponse>()
            .ForMember(d => d.IsOnline, o => o.MapFrom(s => s.Connections.Any()))
            .ForMember(d => d.MessageUnread, o => o.MapFrom(s => s.MessagesSent.Where(x => !x.IsReaded).Count()));
    }
}
