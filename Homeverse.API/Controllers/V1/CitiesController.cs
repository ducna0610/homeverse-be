using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Homeverse.Application.Interfaces;

namespace Homeverse.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ILogger<CitiesController> _logger;
        private readonly ICityService _cityService;
        private readonly ICacheService _cacheService;

        public CitiesController
        (
            ILogger<CitiesController> logger, 
            ICityService cityService,
            ICacheService cacheService
        )
        {
            _logger = logger;
            _cityService = cityService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CityResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var cacheData = await _cacheService.GetDataAsync<IEnumerable<CityResponse>>("cities");
                if (cacheData != null)
                {
                    return Ok(cacheData);
                }
                var response = await _cityService.GetCitiesAsync();

                if (response.Count() == 0)
                {
                    return NotFound();
                }
                await _cacheService.SetDataAsync("cities", response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The method {nameof(CityService.GetCitiesAsync)} caused an exception", ex);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var cacheData = await _cacheService.GetDataAsync<IEnumerable<CityResponse>>("cities");
                if (cacheData != null)
                {
                    var filteredData = cacheData.FirstOrDefault(x => x.Id == id);
                    if (filteredData != null)
                    {
                        return Ok(filteredData);
                    }
                }

                var response = await _cityService.GetCityByIdAsync(id);

                if (response.Id == 0)
                {
                    return NotFound();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The method {nameof(CityService.GetCityByIdAsync)} caused an exception", ex);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CityResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add(CityRequest request)
        {
            try
            {
                var response = await _cityService.AddCityAsync(request);
                await _cacheService.RemoveDataAsync("cities");

                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch
            (Exception ex)
            {
                _logger.LogError($"The method {nameof(CityService.AddCityAsync)} caused an exception", ex);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, CityRequest request)
        {
            try
            {
                var response = await _cityService.UpdateCityAsync(id, request);
                await _cacheService.RemoveDataAsync("cities");

                return Ok(response);
            }
            catch
            (Exception ex)
            {
                _logger.LogError($"The method {nameof(CityService.UpdateCityAsync)} caused an exception", ex);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _cityService.DeleteCityAsync(id);
                await _cacheService.RemoveDataAsync("cities");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"The method {nameof(CityService.DeleteCityAsync)} caused an exception", ex);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
