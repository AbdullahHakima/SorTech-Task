using GeoGuard.Domain.Common;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Entities;
using GeoGuard.Domain.ValueObjects;

namespace GeoGuard.Domain.Interfaces;

public interface IBlockedCountryRepository
{
    Task<Result<BlockedCountry>> AddAsync(BlockedCountry blockedCountry);
    Task<Result<BlockedCountry>> RemoveAsync(CountryCode countryCode);
    Task<Result<BlockedCountry>> GetByCodeAsync(CountryCode code);
    Task<Result<PagedResult<BlockedCountry>>> GetAllAsync(string? countryName, CountryCode? countryCode,
                                                          int pageNumber = 1, int pageSize = 20);

}
