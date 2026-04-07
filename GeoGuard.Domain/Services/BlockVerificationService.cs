using GeoGuard.Domain.Common;
using GeoGuard.Domain.Entities;
using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.ValueObjects;
using System.Net;

namespace GeoGuard.Domain.Services;

public class BlockVerificationService:IBlockVerificationService
{
    private readonly IBlockedCountryRepository _countryRepository;
    private readonly IBlockedAttemptRepository _attemptRepository;
    private readonly IIpLookupService _lookupService;

    public BlockVerificationService(IBlockedCountryRepository countryRepository,
        IBlockedAttemptRepository attemptRepository, IIpLookupService lookupService)
    {
        _countryRepository = countryRepository;
        _attemptRepository = attemptRepository;
        _lookupService = lookupService;
    }

    public async Task<Result<bool>> VerifyAccessAsync(IPAddress ipAddress, UserAgent userAgent)
    {
        var geoResponse = await _lookupService.GetCountryCodeAsync(ipAddress);
        if (!geoResponse.IsSuccess)
            return Result<bool>.NotFound($"the country code for ip:{ipAddress} not found.");

        var countryCode = new CountryCode(geoResponse.Value!.CountryCode2);
        var blockedCountryResult = await _countryRepository.GetByCodeAsync(countryCode);
        if (!blockedCountryResult.IsSuccess)
        {
            await _attemptRepository.AddLogAsync(BlockedAttemptLog.Create(countryCode, userAgent, ipAddress, DateTime.UtcNow, false));
            return Result<bool>.Success(false);

        }
        var blockedCountry = blockedCountryResult.Value!;
        if (blockedCountry.ExpirationTime.HasValue && blockedCountry.ExpirationTime <= DateTime.UtcNow)
        {
            await _attemptRepository.AddLogAsync(BlockedAttemptLog.Create(countryCode, userAgent, ipAddress, DateTime.UtcNow, false));
            return Result<bool>.Success(false);
        }

        await _attemptRepository.AddLogAsync(BlockedAttemptLog.Create(countryCode, userAgent, ipAddress, DateTime.UtcNow, true));
        return Result<bool>.Success(true);

    }
}
