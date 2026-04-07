using GeoGuard.Domain.Interfaces;
using GeoGuard.Infrastructure.Repositories;
using GeoGuard.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GeoGuard.Infrastructure;

public static class dependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IBlockedCountryRepository, InMemoryBlockedCountryRepository>();
        services.AddSingleton<IBlockedAttemptRepository, InMemoryBlockedAttemptRepository>();
        services.AddHttpClient<IIpLookupService, IpGeolocationService>();
        return services;
    }
}
