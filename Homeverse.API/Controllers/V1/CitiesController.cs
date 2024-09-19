using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Homeverse.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ILogger<CitiesController> _logger;
        private readonly ICityService _cityService;

        public CitiesController(ILogger<CitiesController> logger, ICityService cityService)
        {
            _logger = logger;
            _cityService = cityService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CityResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var cities = await _cityService.GetCitiesAsync();

                if (cities.Count() == 0)
                {
                    return NotFound();
                }

                return Ok(cities);
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
                var city = await _cityService.GetCityByIdAsync(id);

                if (city.Id == 0)
                {
                    return NotFound();
                }

                return Ok(city);
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
                var city = await _cityService.AddCityAsync(request);

                return CreatedAtAction(nameof(GetById), new { id = city.Id }, city);
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
                var city = await _cityService.UpdateCityAsync(id, request);

                return Ok(city);
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
