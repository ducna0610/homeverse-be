using Asp.Versioning;
using Homeverse.Application.DTOs.Requests;
using Homeverse.Application.DTOs.Responses;
using Homeverse.Application.Interfaces;
using Homeverse.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Homeverse.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ILogger<ContactsController> _logger;
    private readonly IContactService _contactService;
    private readonly ICacheService _cacheService;

    public ContactsController
    (
        ILogger<ContactsController> logger,
        IContactService contactService,
        ICacheService cacheService
    )
    {
        _logger = logger;
        _contactService = contactService;
        _cacheService = cacheService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ContactResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var cacheData = await _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts");
            if (cacheData != null)
            {
                return Ok(cacheData);
            }

            var response = await _contactService.GetContactsAsync();
            if (response.Count() == 0)
            {
                return NotFound();
            }
            await _cacheService.SetDataAsync("contacts", response);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(ContactService.GetContactsAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var cacheData = await _cacheService.GetDataAsync<IEnumerable<ContactResponse>>("contacts");
            if (cacheData != null)
            {
                var filteredData = cacheData.FirstOrDefault(x => x.Id == id);
                if (filteredData != null)
                {
                    return Ok(filteredData);
                }
            }

            var response = await _contactService.GetContactByIdAsync(id);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(ContactService.GetContactByIdAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Add(ContactRequest request)
    {
        try
        {
            var response = await _contactService.AddContactAsync(request);
            await _cacheService.RemoveDataAsync("contacts");

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch
        (Exception ex)
        {
            _logger.LogError($"The method {nameof(ContactService.AddContactAsync)} caused an exception", ex);
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
            await _contactService.DeleteContactAsync(id);
            await _cacheService.RemoveDataAsync("contacts");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"The method {nameof(ContactService.DeleteContactAsync)} caused an exception", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
