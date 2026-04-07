using System.Text.Json.Serialization;

namespace GeoGuard.Domain.DTOs;

public record GeolocationResponse
{
    [JsonPropertyName("country_code2")]
    public string CountryCode2 { get; set; } = null!;
}
