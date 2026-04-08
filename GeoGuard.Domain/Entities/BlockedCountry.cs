using GeoGuard.Domain.ValueObjects;

namespace GeoGuard.Domain.Entities;

public class BlockedCountry
{
    public string Name { get; protected set; } = null!;
    public CountryCode CountryCode { get; private set; } = null!;
    public DateTime BlockedAt { get; private set; }
    public DateTime? ExpirationTime { get; private set; }
    private BlockedCountry() { }

    public static BlockedCountry Create(string name, CountryCode countryCode, DateTime blockedAt,
           DateTime? expirationTime) => new()
           {
               Name = name,
               CountryCode = countryCode,
               BlockedAt = blockedAt,
               ExpirationTime = expirationTime
           };

}
