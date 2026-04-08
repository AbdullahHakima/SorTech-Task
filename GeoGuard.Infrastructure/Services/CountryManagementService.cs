using GeoGuard.Domain.Common;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Entities;
using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.Services;
using GeoGuard.Domain.ValueObjects;
using GeoGuard.Infrastructure.Exceptions;

namespace GeoGuard.Infrastructure.Services;

public class CountryManagementService : ICountryManagementService
{
    private readonly IBlockedCountryRepository _countryRepository;
    private readonly ICountryNameService _countryNameService;

    public CountryManagementService(IBlockedCountryRepository countryRepository, ICountryNameService countryNameService)
    {
        _countryRepository = countryRepository;
        _countryNameService = countryNameService;
    }

    public async Task<Result<BlockedCountry>> BlockCountryAsync(string countryCodeString)
    {
        var countryCode = new CountryCode(countryCodeString);
        var countryName = await _countryNameService.GetName(countryCode);
        var blockedCountry = BlockedCountry.Create(countryName, countryCode, DateTime.UtcNow, null);
        return await _countryRepository.AddAsync(blockedCountry);
    }

    public async Task<Result<PagedResult<BlockedCountry>>> BlockedListAsync(string? countryName, string? countryCodeString, int pageNumber = 1, int pageSize = 20)
    {
        pageSize = pageSize > 50 ? Math.Clamp(pageSize, 20, 50) : pageSize;
        if (!string.IsNullOrEmpty(countryName) && !string.IsNullOrEmpty(countryCodeString))
            throw new GeoGuardSeviceException("Can not seach by country code and country name at same time");
        if (!string.IsNullOrEmpty(countryCodeString))
        {
            var countryCode = new CountryCode(countryCodeString);
            return await _countryRepository.GetAllAsync(null, countryCode, pageNumber, pageSize);
        }
        return await _countryRepository.GetAllAsync(countryName, null, pageNumber, pageSize);
    }

    public async Task<Result<BlockedCountry>> TemporalBlockCountryAsync(string countryCodeString, int durationMinutes)
    {
        if (durationMinutes < 1 || durationMinutes > 1440)
            throw new GeoGuardSeviceException("Duration must be between 1 and 1440 minutes (24 hours).");
        if (string.IsNullOrEmpty(countryCodeString))
            throw new GeoGuardSeviceException("Country code is required");
        var countryCode = new CountryCode(countryCodeString);
        var expiration = DateTime.UtcNow.AddMinutes(durationMinutes);
        string countryName = await _countryNameService.GetName(countryCode);
        var blockedCountry = BlockedCountry.Create(countryName, countryCode, DateTime.UtcNow, expiration);
        return await _countryRepository.AddAsync(blockedCountry);

    }

    public async Task<Result<BlockedCountry>> UnblockCountryAsync(string countryCodeString)
    {
        var countryCode = new CountryCode(countryCodeString);
        return await _countryRepository.RemoveAsync(countryCode);
    }
}
