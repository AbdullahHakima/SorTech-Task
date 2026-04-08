using GeoGuard.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeoGuard.Infrastructure.BackgroundServices;

public class TemporalBlockExpirationWorker:BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TemporalBlockExpirationWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    public TemporalBlockExpirationWorker(IServiceProvider serviceProvider,
                            ILogger<TemporalBlockExpirationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Temporal Block Expiration Worker started.");
        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_interval, stoppingToken);
            await CleanupExpiredBlocksAsync();
        }
    }

    private async Task CleanupExpiredBlocksAsync()
    {
        _logger.LogInformation("Running expired temporal block cleanup at {Time}", DateTime.UtcNow);
         using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IBlockedCountryRepository>();
        var allResult = await repository.GetAllAsync(null, null, 1, int.MaxValue);
        if (!allResult.IsSuccess) 
            return;
        var expired = allResult.Value!.Items
                    .Where(bc => bc.ExpirationTime.HasValue 
                    && bc.ExpirationTime <= DateTime.UtcNow).ToList();
        foreach(var country in expired)
        {
            await repository.RemoveAsync(country.CountryCode);
            _logger.LogInformation("Expired temporal block removed for country: {Code}",
                                    country.CountryCode.Value);
        }
    }
}
