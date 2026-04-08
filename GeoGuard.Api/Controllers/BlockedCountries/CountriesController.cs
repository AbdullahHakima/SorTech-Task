using GeoGuard.Api.Controllers.BlockedCountries.DTOs;
using GeoGuard.Api.Controllers.Countries.DTOs;
using GeoGuard.Api.Extensions;
using GeoGuard.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoGuard.Api.Controllers.Countries;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ICountryManagementService _countryManagement;

    public CountriesController(ICountryManagementService countryManagement)
    {
        _countryManagement = countryManagement;
    }

    [HttpPost("block")]
    public async Task<IActionResult> AddToBlockList([FromBody] string countryCodeString)
             => (await _countryManagement.BlockCountryAsync(countryCodeString)).ToActionResult(this);

    [HttpDelete("block/{countryCode}")]
    public async Task<IActionResult> RemoveFromBlockedList([FromRoute] string countryCode)
            => (await _countryManagement.UnblockCountryAsync(countryCode)).ToActionResult(this);
    [HttpGet("blocked")]
    public async Task<IActionResult> GetBlockedList([FromQuery] GetBlockedListRequest request)
    => (await _countryManagement.BlockedListAsync(request.CountryName, request.CountryCode,
                        request.PageNumber, request.PageSize)).ToActionResult(this);
    [HttpPost("temporal-block")]
    public async Task<IActionResult> AddTemporalBlock([FromBody] TemporalBlockRequest request)
    => (await _countryManagement.TemporalBlockCountryAsync(request.CountryCode, request.DurationMinutes))
                               .ToActionResult(this);



}
