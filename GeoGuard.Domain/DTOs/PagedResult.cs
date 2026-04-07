namespace GeoGuard.Domain.DTOs;

public record PagedResult<T>(
    int TotalCount,
    int TotalPages,
    int CurrentPage,
    IEnumerable<T> Items);
