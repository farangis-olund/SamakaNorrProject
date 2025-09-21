using Infrastructure.Entities;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services;

public class CleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var searchRepo = scope.ServiceProvider.GetRequiredService<SearchRequestRepository>();

                // Get response
                var result = await searchRepo.GetAllAsync(r => r.DepartureTime < DateTime.Today);

                if (result.ContentResult is List<SearchRequestEntity> oldRequests && oldRequests.Any())
                {
                    foreach (var request in oldRequests)
                    {
                        await searchRepo.RemoveAsync(x => x.Id == request.Id);
                    }
                }
            }

            // Run once per month (30 days)
            await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
        }
    }

}
