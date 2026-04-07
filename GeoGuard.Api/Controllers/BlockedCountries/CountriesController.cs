using GeoGuard.Api.Extensions;
using GeoGuard.Domain.Controllers.BlockedCountries.DTOs;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GeoGuard.Api.Controllers.Countries;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly IBlockedCountryRepository _countryRepository;

    public CountriesController(IBlockedCountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }
    [HttpPost("block")]
    public async Task<IActionResult> AddToBlockList( [FromQuery] AddCountryToBlockedListRequest request)
             => await _countryRepository.AddAsync(request).Result.ToActionResult(this);

    [HttpDelete("block/{countryCode}")]
    public async Task<IActionResult> RemoveFromBlockedList([FromRoute] string countryCode)
            => await _countryRepository.RemoveAsync(countryCode).Result.ToActionResult(this);
    [HttpGet("blocked")]
    public async Task<IActionResult> GetBlockedList([FromQuery] GetBlockedListRequest request)
    => await _countryRepository.GetAllAsync(request).Result.ToActionResult(this);
   
}
