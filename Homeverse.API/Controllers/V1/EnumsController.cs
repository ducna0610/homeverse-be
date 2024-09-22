using Asp.Versioning;
using Homeverse.Application.Interfaces;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Homeverse.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EnumsController : ControllerBase
    {
        private readonly ILogger<EnumsController> _logger;
        private readonly IEnumService _enumService;
        private readonly ICacheService _cacheService;

        public EnumsController
        (
            ILogger<EnumsController> logger,
            IEnumService enumService,
            ICacheService cacheService
        )
        {
            _logger = logger;
            _enumService = enumService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("category")]
        public async Task<IActionResult> GetCategoryEnum()
        {
            try
            {
                var cacheData = await _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("categories");
                if (cacheData != null)
                {
                    return Ok(cacheData);
                }

                var response = _enumService.GetCaegoryEnum();
                if (response.Count() == 0)
                {
                    return NotFound();
                }
                await _cacheService.SetDataAsync("categories", response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The method {nameof(EnumService.GetCaegoryEnum)} caused an exception", ex);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet]
        [Route("furnish")]
        public async Task<IActionResult> GetFurnishEnum()
        {
            try
            {
                var cacheData = await _cacheService.GetDataAsync<IEnumerable<KeyValuePair<int, string>>>("furnishes");
                if (cacheData != null)
                {
                    return Ok(cacheData);
                }

                var response = _enumService.GetFurnishEnum();
                if (response.Count() == 0)
                {
                    return NotFound();
                }
                await _cacheService.SetDataAsync("furnishes", response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The method {nameof(EnumService.GetFurnishEnum)} caused an exception", ex);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
