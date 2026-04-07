namespace GeoGuard.Domain.ValueObjects;

public record UserAgent
{
    public string Value { get; } = null!;
    public UserAgent(string value)
    {
        if(string.IsNullOrEmpty(value))
        {
            Value = "UNKNOWN";
            return;
        }
        Value = value.Length > 500 ? value.Substring(0, 500) : value;
    }
    public override string ToString()
                        => Value;
}
