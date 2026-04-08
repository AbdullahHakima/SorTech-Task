using GeoGuard.Domain.Common;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Entities;

namespace GeoGuard.Domain.Services;

public interface ICountryManagementService
{
    Task<Result<BlockedCountry>> BlockCountryAsync(string countryCode);
    Task<Result<BlockedCountry>> TemporalBlockCountryAsync(string countryCodeString, int durationMinutes);
    Task<Result<BlockedCountry>> UnblockCountryAsync(string countryCode);
    Task<Result<PagedResult<BlockedCountry>>> BlockedListAsync(string? countryName, string? countryCode,
                                                               int pageNumber = 1, int pageSize = 20);
}
