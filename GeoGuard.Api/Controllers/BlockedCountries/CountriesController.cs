using GeoGuard.Api.Controllers.BlockedCountries.DTOs;
using GeoGuard.Api.Controllers.Countries.DTOs;
using GeoGuard.Api.Extensions;
using GeoGuard.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoGuard.Api.Controllers.Countries;

/// <summary>
/// Manages the strict IP blocking list for specific countries.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ICountryManagementService _countryManagement;

    public CountriesController(ICountryManagementService countryManagement)
    {
        _countryManagement = countryManagement;
    }

    /// <summary>
    /// Adds a given country code to the permanent block list.
    /// </summary>
    /// <param name="countryCodeString">The 2-character ISO country code (e.g., US, EG).</param>
    /// <returns>A confirmation of the newly blocked country.</returns>
    /// <response code="200">If the country was successfully blocked or is already on the permanent list.</response>
    /// <response code="400">If the country code format is invalid.</response>
    [HttpPost("block")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToBlockList([FromBody] string countryCodeString)
             => (await _countryManagement.BlockCountryAsync(countryCodeString)).ToActionResult(this);

    /// <summary>
    /// Removes a country from the active block list (whether permanent or temporal).
    /// </summary>
    /// <param name="countryCode">The 2-character ISO country code.</param>
    /// <returns>A confirmation that the country was removed or wasn't found.</returns>
    /// <response code="200">Successfully unblocked the country.</response>
    [HttpDelete("block/{countryCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveFromBlockedList([FromRoute] string countryCode)
            => (await _countryManagement.UnblockCountryAsync(countryCode)).ToActionResult(this);

    /// <summary>
    /// Retrieves a paginated list of blocked countries, with optional filters.
    /// </summary>
    /// <param name="request">Contains pagination logic and optional filters (e.g. searching by country code or name).</param>
    /// <returns>A paginated list of BlockedCountry objects.</returns>
    /// <response code="200">Successfully retrieved the block list.</response>
    /// <response code="400">If pagination values are invalid.</response>
    [HttpGet("blocked")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBlockedList([FromQuery] GetBlockedListRequest request)
    => (await _countryManagement.BlockedListAsync(request.CountryName, request.CountryCode,
                        request.PageNumber, request.PageSize)).ToActionResult(this);

    /// <summary>
    /// Temporarily blocks a country for a specific duration (in minutes).
    /// </summary>
    /// <param name="request">Contains the target country code and duration in minutes (between 1 and 86400).</param>
    /// <returns>Confirmation of the temporal block mapping.</returns>
    /// <response code="200">The temporal block has been successfully established.</response>
    /// <response code="400">If the country code is invalid, or minutes are out of bounds.</response>
    /// <response code="409">If the target country is already permanently blocked.</response>
    [HttpPost("temporal-block")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddTemporalBlock([FromBody] TemporalBlockRequest request)
    => (await _countryManagement.TemporalBlockCountryAsync(request.CountryCode, request.DurationMinutes))
                               .ToActionResult(this);

}
