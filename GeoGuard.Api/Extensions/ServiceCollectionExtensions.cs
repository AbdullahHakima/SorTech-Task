using System.Threading.RateLimiting;
using GeoGuard.Api.Middlewares;

namespace GeoGuard.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiProtections(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = 429;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetSlidingWindowLimiter(ip, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 4,
                    AutoReplenishment = true
                });
            });
        });

        return services;
    }
}
