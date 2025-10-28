
using BusinessSharkService.Constants;
using BusinessSharkService.Handlers;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.CoreServices
{
    public class CalculationService : BackgroundService
    {
        private readonly ILogger<CalculationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CalculationService(IConfiguration configuration, ILogger<CalculationService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }
 
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service is working.");

            int delayMs = Convert.ToInt32(TimeSpan.FromMinutes(_configuration.GetValue<int>(ConfigKey.CalculatePeriodMinutes)).TotalMilliseconds);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var worldHandler = scope.ServiceProvider.GetRequiredService<WorldHandler>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation($"Started world calculation");

                    try
                    {
                        await worldHandler.Calculate(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                    await Task.Delay(delayMs, stoppingToken);
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
