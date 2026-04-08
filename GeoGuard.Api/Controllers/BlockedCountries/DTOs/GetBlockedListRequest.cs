namespace GeoGuard.Api.Controllers.Countries.DTOs;

public record GetBlockedListRequest(string? CountryName, string? CountryCode, int PageNumber = 1, int PageSize = 20);
