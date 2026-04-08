namespace GeoGuard.Api.Controllers.BlockedCountries.DTOs;

public record TemporalBlockRequest(string CountryCode, int DurationMinutes);
