using GeoGuard.Domain.Common;
using GeoGuard.Domain.DTOs;
using GeoGuard.Domain.Interfaces;
using GeoGuard.Infrastructure.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace GeoGuard.Infrastructure.Services;

public class IpGeolocationService : IIpLookupService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions;

    public IpGeolocationService(IHttpClientFactory httpClient,IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["ipgeolocation.io:ApiKey"]
            ?? throw new GeoGuardInfrastructureException("API Key is missing!");
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<Result<GeolocationResponse>> GetCountryCodeAsync(IPAddress ipAddress)
    {
        string url = $"https://api.ipgeolocation.io/ipgeo?apiKey={_apiKey}&ip={ipAddress}";
        var client = _httpClient.CreateClient("GeoGurad");
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GeolocationResponse>(json, _jsonOptions);

        if (result is null)
            return Result<GeolocationResponse>.NotFound(
                    $"No country code associated the {ipAddress} may be worng or try again later.");
        return Result<GeolocationResponse>.Success(result);

    }
}
