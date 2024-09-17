using AutoMapper;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Domain.Entities;
using Homeverse.Domain.Interfaces;

namespace Homeverse.Application.Services;

public interface ICityService
{
    Task<IEnumerable<CityResponse>> GetCitiesAsync();
    Task<CityResponse> GetCityByIdAsync(int id);
    Task<CityResponse> AddCityAsync(CityRequest request);
    Task<CityResponse> UpdateCityAsync(int id, CityRequest request);
    Task DeleteCityAsync(int id);
}

public class CityService : ICityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICityRepository _cityRepository;

    public CityService(IUnitOfWork unitOfWork, IMapper mapper, ICityRepository cityRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cityRepository = cityRepository;
    }

    public async Task<IEnumerable<CityResponse>> GetCitiesAsync()
    {
        var cities = await _cityRepository.GetCitiesAsync();

        return _mapper.Map<IEnumerable<CityResponse>>(cities);
    }

    public async Task<CityResponse> GetCityByIdAsync(int id)
    {
        var city = await _cityRepository.GetCityByIdAsync(id);

        return _mapper.Map<CityResponse>(city);
    }

    public async Task<CityResponse> AddCityAsync(CityRequest request)
    {
        var city = _mapper.Map<City>(request);

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _cityRepository.AddCityAsync(city);
        });

        return _mapper.Map<CityResponse>(city);
    }

    public async Task<CityResponse> UpdateCityAsync(int id, CityRequest request)
    {
        var oldCity = await _cityRepository.GetCityByIdAsync(id);

        var city = _mapper.Map<City>(request);
        city.Id = id;
        city.CreatedAt = oldCity.CreatedAt;
        city.UpdatedAt = DateTimeOffset.Now;

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _cityRepository.UpdateCityAsync(city);
        });

        return _mapper.Map<CityResponse>(city);
    }

    public async Task DeleteCityAsync(int id)
    {
        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            await _cityRepository.DeleteCityAsync(id);
        });
    }
}