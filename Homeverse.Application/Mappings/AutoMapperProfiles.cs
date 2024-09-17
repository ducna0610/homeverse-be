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
    }
}
