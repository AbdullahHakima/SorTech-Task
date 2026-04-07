using GeoGuard.Domain.Common;
using GeoGuard.Domain.DTOs;
using System.Net;

namespace GeoGuard.Domain.Interfaces;

public interface IIpLookupService
{
    Task<Result<GeolocationResponse>> GetCountryCodeAsync(IPAddress ipAddress);
}
