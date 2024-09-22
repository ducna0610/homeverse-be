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

public class CityServiceTests
{
    private readonly Fixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICityRepository _cityRepository;
    private readonly ICityService _sut;

    public CityServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Customize<City>(e => e.With(x => x.Properties, new List<Property>()));
        var context = MockDbContext.CreateMockDbContext();
        _unitOfWork = new UnitOfWork(context);
        _mapper = A.Fake<IMapper>();
        _cityRepository = A.Fake<ICityRepository>();
        _sut = new CityService(_unitOfWork, _mapper, _cityRepository);
    }

    [Fact]
    public async Task GetCitiesAsync_WhenSuccessful_ShouldReturnCities()
    {
        // Arrange
        _fixture.Customize<City>(e => e.With(x => x.Properties, new List<Property>()));
        var cities = _fixture.CreateMany<City>(3).ToList();
        var response = _fixture.CreateMany<CityResponse>(3).ToList();
        A.CallTo(() => _cityRepository.GetCitiesAsync()).Returns(cities);
        A.CallTo(() => _mapper.Map<IEnumerable<CityResponse>>(A<IEnumerable<City>>._)).Returns(response);

        // Act
        var actual = await _sut.GetCitiesAsync();

        // Assert
        A.CallTo(() => _cityRepository.GetCitiesAsync()).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<IEnumerable<CityResponse>>(actual);
        Assert.Equal(cities.Count(), actual.Count());
    }

    [Fact]
    public async Task GetCityByIdAsync_WhenSuccessful_ShouldReturnCity()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var city = _fixture.Create<City>();
        var response = _fixture.Create<CityResponse>();
        A.CallTo(() => _cityRepository.GetCityByIdAsync(id)).Returns(city);
        A.CallTo(() => _mapper.Map<CityResponse>(A<City>._)).Returns(response);

        // Act
        var actual = await _sut.GetCityByIdAsync(id);

        // Assert
        A.CallTo(() => _cityRepository.GetCityByIdAsync(A<int>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<CityResponse>(actual);
    }

    [Fact]
    public async Task AddCityAsync_WhenSuccessful_ShouldAddAndReturnCity()
    {
        // Arrange
        var request = _fixture.Create<CityRequest>();
        var city = _fixture.Create<City>();
        var response = _fixture.Create<CityResponse>();
        A.CallTo(() => _mapper.Map<City>(A<CityRequest>._)).Returns(city);
        A.CallTo(() => _cityRepository.AddCityAsync(city));
        A.CallTo(() => _mapper.Map<CityResponse>(A<City>._)).Returns(response);

        // Act
        var actual = await _sut.AddCityAsync(request);

        // Assert
        A.CallTo(() => _cityRepository.AddCityAsync(A<City>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<CityResponse>(actual);
    }

    [Fact]
    public async Task UpdateCityAsync_WhenSuccessful_ShouldUpdateAndReturnCity()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CityRequest>();
        var city = _fixture.Create<City>();
        var response = _fixture.Create<CityResponse>();
        A.CallTo(() => _mapper.Map<City>(A<CityRequest>._)).Returns(city);
        A.CallTo(() => _cityRepository.UpdateCityAsync(city));
        A.CallTo(() => _mapper.Map<CityResponse>(A<City>._)).Returns(response);

        // Act
        var actual = await _sut.UpdateCityAsync(id, request);

        // Assert
        A.CallTo(() => _cityRepository.UpdateCityAsync(A<City>._)).MustHaveHappenedOnceExactly();
        Assert.IsAssignableFrom<CityResponse>(actual);
    }

    [Fact]
    public async Task DeleteCityAsync_WhenSuccessful_ShouldDeleteCity()
    {
        // Arrange
        var id = _fixture.Create<int>();
        A.CallTo(() => _cityRepository.DeleteCityAsync(id));

        // Act
        await _sut.DeleteCityAsync(id);

        // Assert
        A.CallTo(() => _cityRepository.DeleteCityAsync(A<int>._)).MustHaveHappenedOnceExactly();
    }
}
