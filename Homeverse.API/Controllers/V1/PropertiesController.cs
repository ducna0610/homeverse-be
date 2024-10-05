using Asp.Versioning;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Interfaces;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Homeverse.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly ILogger<PropertiesController> _logger;
    private readonly IPropertyService _propertyService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;

    public PropertiesController
    (
        ILogger<PropertiesController> logger,
        IPropertyService propertyService,
        ICurrentUserService currentUserService,
        ICacheService cacheService
    )
    {
        _logger = logger;
        _propertyService = propertyService;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PropertyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var cacheData = await _cacheService.GetDataAsync<IEnumerable<PropertyResponse>>("properties");
            if (cacheData != null)
            {
                return Ok(cacheData);
            }

            var response = await _propertyService.GetAllPropertiesAsync();
            if (response.Count() == 0)
            {
                return NotFound();
            }
            await _cacheService.SetDataAsync("properties", response);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.GetAllPropertiesAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("user")]
    [ProducesResponseType(typeof(IEnumerable<PropertyDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetForUser()
    {
        try
        {
            var response = await _propertyService.GetAllPropertiesForUserAsync();
            if (response.Count() == 0)
            {
                return NotFound();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.GetAllPropertiesForUserAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    [Route("list")]
    [ProducesResponseType(typeof(IEnumerable<PropertyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActive()
    {
        try
        {
            var response = await _propertyService.GetPropertiesAsync();
            if (response.Count() == 0)
            {
                return NotFound();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.GetPropertiesAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    [Route("detail/{id}")]
    [ProducesResponseType(typeof(PropertyDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var response = await _propertyService.GetPropertyByIdAsync(id);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.GetPropertyByIdAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PropertyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Add([FromForm] PropertyRequest request)
    {
        try
        {
            var response = await _propertyService.AddPropertyAsync(request);
            await _cacheService.RemoveDataAsync("properties");

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.AddPropertyAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut]
    [Route("{id}")]
    [ProducesResponseType(typeof(PropertyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromForm] PropertyRequest request)
    {
        try
        {
            var property = await _propertyService.UpdatePropertyAsync(id, request);
            await _cacheService.RemoveDataAsync("properties");

            return Ok(property);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.UpdatePropertyAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _propertyService.DeletePropertyAsync(id);
            await _cacheService.RemoveDataAsync("properties");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.DeletePropertyAsync)} caused an exception", ex);
        }
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [Authorize]
    [HttpPost("set-primary-photo/{propId}/{photoId}")]
    [ProducesResponseType(typeof(PhotoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SetPrimaryPhoto(int propId, string photoPublicId)
    {
        try
        {
            var property = await _propertyService.GetPropertyByIdAsync(propId);
            if (property.PostedBy.Id != _currentUserService.UserId)
            {
                return BadRequest("You are not authorised to change the photo");
            }
            if (property.Id == 0)
            {
                return BadRequest("No such property or photo exists");
            }

            var photo = property.Photos.FirstOrDefault(p => p.PublicId == photoPublicId);
            if (photo == null)
            {
                return BadRequest("No such property or photo exists");
            }
            if (photo.IsPrimary)
            {
                return BadRequest("This is already a primary photo");
            }

            var photos = await _propertyService.SetPrimaryPhotoAsync(photoPublicId);

            return Ok(photos);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.SetPrimaryPhotoAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("bookmarks")]
    [ProducesResponseType(typeof(IEnumerable<PropertyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBookmarks()
    {
        try
        {
            var response = await _propertyService.GetBookmarksAsync();
            if (response.Count() == 0)
            {
                return NotFound();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.GetBookmarksAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("add-bookmark/{propId}")]
    [ProducesResponseType(typeof(IEnumerable<PropertyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddBookmark(int propId)
    {
        try
        {
            var property = await _propertyService.GetPropertyByIdAsync(propId);
            if (property.Id == 0)
            {
                return BadRequest("Property does not exists");
            }

            var bookmark = (await _propertyService.GetBookmarksAsync()).FirstOrDefault(x => x.Id == propId);
            if (bookmark != null)
            {
                return BadRequest("Property already bookmark");
            }

            await _propertyService.AddBookmarkAsync(propId);

            return await GetBookmarks();
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.AddBookmarkAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpDelete("delete-bookmark/{propId}")]
    [ProducesResponseType(typeof(IEnumerable<PropertyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBookmark(int propId)
    {
        try
        {
            await _propertyService.DeleteBookmarkAsync(propId);

            return await GetBookmarks();
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(PropertyService.DeleteBookmarkAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
