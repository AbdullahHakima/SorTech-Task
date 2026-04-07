using GeoGuard.Domain.Interfaces;
using GeoGuard.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GeoGuard.Domain;

public static class dependencyInjection
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IBlockVerificationService, BlockVerificationService>();
        return services;
    }
}
