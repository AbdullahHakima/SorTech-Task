using System.Text.Json.Serialization;

namespace GeoGuard.Domain.DTOs;

public class GeolocationResponse
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;

    [JsonPropertyName("country_code2")]
    public string CountryCode2 { get; set; } = string.Empty;

    [JsonPropertyName("country_name")]
    public string CountryName { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public string Latitude { get; set; } = string.Empty;

    [JsonPropertyName("longitude")]
    public string Longitude { get; set; } = string.Empty;

    [JsonPropertyName("isp")]
    public string Isp { get; set; } = string.Empty;

    [JsonPropertyName("organization")]
    public string Organization { get; set; } = string.Empty;

    [JsonPropertyName("is_eu")]
    public bool IsEU { get; set; }

    [JsonPropertyName("connection_type")]
    public string ConnectionType { get; set; } = string.Empty;
}
