using AutoMapper;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Domain.Entities;
using Homeverse.Domain.Enums;

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
            .ForMember(d => d.Name, o => o.MapFrom(x => x.UserName))
            .ForMember(d => d.PasswordHash, o => o.MapFrom(x => Convert.FromBase64String("")))
            .ForMember(d => d.PasswordSalt, o => o.MapFrom(x => Convert.FromBase64String("")));
        CreateMap<User, UserResponse>();
        CreateMap<User, FriendResponse>()
            .ForMember(d => d.IsOnline, o => o.MapFrom(x => x.Connections.Any()))
            .ForMember(d => d.MessageUnread, o => o.MapFrom(x => x.MessagesSent.Where(y => !y.IsReaded).Count()));

        CreateMap<PropertyRequest, Property>();
        CreateMap<Property, PropertyResponse>()
            .ForMember(d => d.City, o => o.MapFrom(x => x.City.Name))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(x => x.Photos.FirstOrDefault(y => y.IsPrimary).ImageUrl))
            .ForMember(d => d.NumberImage, o => o.MapFrom(x => x.Photos.Count()))
            .ForMember(d => d.Category, o => o.MapFrom(x => EnumExtension.GetDescription(x.CategoryId)))
            .ForMember(d => d.Furnish, o => o.MapFrom(x => EnumExtension.GetDescription(x.FurnishId)))
            .ForMember(d => d.PostedBy, o => o.MapFrom(x => x.User));
        CreateMap<Property, PropertyDetailResponse>()
            .ForMember(d => d.City, o => o.MapFrom(x => x.City.Name))
            .ForMember(d => d.Category, o => o.MapFrom(x => EnumExtension.GetDescription(x.CategoryId)))
            .ForMember(d => d.Furnish, o => o.MapFrom(x => EnumExtension.GetDescription(x.FurnishId)))
            .ForMember(d => d.PostedBy, o => o.MapFrom(x => x.User))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(x => x.Photos.FirstOrDefault(x => x.IsPrimary == true).ImageUrl));

        CreateMap<Photo, PhotoResponse>();

        CreateMap<MessageRequest, Message>();
        CreateMap<Message, MessageResponse>();
    }
}
