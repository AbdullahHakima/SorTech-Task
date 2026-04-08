using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.Services;
using GeoGuard.Infrastructure.BackgroundServices;
using GeoGuard.Infrastructure.Repositories;
using GeoGuard.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GeoGuard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IBlockedCountryRepository, InMemoryBlockedCountryRepository>();
        services.AddSingleton<IBlockedAttemptRepository, InMemoryBlockedAttemptRepository>();
        services.AddSingleton<ICountryNameService, CountryNameService>();
        services.AddScoped<ICountryManagementService, CountryManagementService>();
        services.AddHttpClient<IIpLookupService, IpGeolocationService>();
        services.AddHostedService<TemporalBlockExpirationWorker>();
        return services;
    }
}
