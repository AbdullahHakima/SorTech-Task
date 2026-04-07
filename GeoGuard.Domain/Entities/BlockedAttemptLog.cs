using GeoGuard.Domain.ValueObjects;
using System.Net;

namespace GeoGuard.Domain.Entities;

public class BlockedAttemptLog
{
    public CountryCode CountryCode { get; private set; } = null!;
    public UserAgent UserAgent { get; private set; } = null!;
    public IPAddress IPAddress { get; private set; } = null!;
    public DateTime TimeStamp { get; private set; }
    public bool IsBlocked { get; private set; }

    private BlockedAttemptLog() { }

    public static BlockedAttemptLog Create(CountryCode countryCode, UserAgent userAgent,
        IPAddress iPAddress, DateTime timeStamp, bool isBlocked) => new()
        {
            CountryCode = countryCode,
            UserAgent = userAgent,
            IPAddress = iPAddress,
            TimeStamp = timeStamp,
            IsBlocked = isBlocked
        };

}
