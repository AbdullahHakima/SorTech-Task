using GeoGuard.Domain.Exceptions;

namespace GeoGuard.Domain.ValueObjects;

public record CountryCode
{
    public string Value { get; } = null!;
    public CountryCode(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new GeoGuardDomainException("Country code can not be empty.");
        if (value.Length < 2 || value.Length > 3)
            throw new GeoGuardDomainException("Country code must be 2 or 3 characters.");
        if (!value.All(char.IsLetter))
            throw new GeoGuardDomainException("Country code must contain only letters");
        Value = value.ToUpperInvariant();
    }
    public override string ToString()
                        => Value;

}
