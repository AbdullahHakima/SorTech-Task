using GeoGuard.Domain.Controllers.BlockedCountries.DTOs;
using GeoGuard.Domain.Common;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Entities;
using GeoGuard.Domain.ValueObjects;

namespace GeoGuard.Domain.Interfaces;

public interface IBlockedCountryRepository
{
    Task<Result<BlockedCountry>> AddAsync(AddCountryToBlockedListRequest request);
    Task<Result<BlockedCountry>> RemoveAsync(string countryCodeString);
    Task<Result<BlockedCountry>> GetByCodeAsync(CountryCode code);
    Task<Result<PagedResult<BlockedCountry>>> GetAllAsync(GetBlockedListRequest request);
}
