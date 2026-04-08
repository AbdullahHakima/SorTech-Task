using GeoGuard.Domain.ValueObjects;

namespace GeoGuard.Domain.Interfaces;

public interface ICountryNameService
{
    Task<string> GetName(CountryCode countryCode);
}
