namespace GeoGuard.Domain.Controllers.BlockedCountries.DTOs;

public record AddCountryToBlockedListRequest(
    string CountryName, string CountryCodeString, DateTime ExpirationDate);
