using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.ValueObjects;
using GeoGuard.Infrastructure.Exceptions;
using System.Globalization;

namespace GeoGuard.Infrastructure.Services;

internal class CountryNameService : ICountryNameService
{
    public async Task<string> GetName(CountryCode countryCode)
    {
        try
        {
            var region = new RegionInfo(countryCode.Value);
            return region.EnglishName;
        }
        catch
        {
            throw new GeoGuardInfrastructureException(
                $"Name for country code:{countryCode} not found");
        }

    }
}

