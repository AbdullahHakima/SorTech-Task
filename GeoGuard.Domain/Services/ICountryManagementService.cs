using GeoGuard.Domain.Common;
using GeoGuard.Domain.Entities;

namespace GeoGuard.Domain.Services;

public interface ICountryManagementService
{
    Task<Result<BlockedCountry>> BlockCountryAsync(string countryCode);
    Task<Result<BlockedCountry>> TemporalBlockCountryAsync(string countryCode, int durationMinutes);
    Task<Result<BlockedCountry>> UnblockCountryAsync(string countryCode);
}
