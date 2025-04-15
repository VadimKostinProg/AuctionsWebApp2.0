using BidMasterOnline.Application.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BidMasterOnline.Application.Services
{
    public class PeriodicHostedService : BackgroundService
    {
        private readonly TimeSpan _period;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PeriodicHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public PeriodicHostedService(IConfiguration configuration, ILogger<PeriodicHostedService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _scopeFactory = scopeFactory;

            _period = TimeSpan.FromSeconds(int.Parse(_configuration["PeiodicHostedProcesses:Period"]!));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("->PeriodicHostedService has started execution.");

            using PeriodicTimer timer = new PeriodicTimer(_period);

            while (!stoppingToken.IsCancellationRequested &&
                  await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using AsyncServiceScope asyncScope = _scopeFactory.CreateAsyncScope();

                    IPeriodicTaskService service = asyncScope.ServiceProvider.GetRequiredService<IPeriodicTaskService>();

                    await service.PerformPeriodicTaskAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"->PeriodicHostedService: error occured while prforming periodic task - {ex.Message}.");
                }
            }

            _logger.LogInformation("->PeriodicHostedService has finished execution.");
        }
    }
}
