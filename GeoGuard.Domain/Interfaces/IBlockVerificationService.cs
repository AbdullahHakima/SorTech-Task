using GeoGuard.Domain.Common;
using GeoGuard.Domain.ValueObjects;
using System.Net;

namespace GeoGuard.Domain.Interfaces;

public interface IBlockVerificationService
{
    Task<Result<bool>> VerifyAccessAsync(IPAddress ipAddress, UserAgent userAgent);

}
