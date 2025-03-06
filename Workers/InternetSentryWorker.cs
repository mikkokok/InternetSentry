
using InternetSentry.Services;

namespace InternetSentry.Workers
{
    public sealed class InternetSentryWorker(ILogger<InternetSentryWorker> logger, TechniService techniService) : BackgroundService
    {
        private readonly ILogger<InternetSentryWorker> _logger = logger;
        private readonly TechniService _techniService = techniService;
        private readonly string _serviceName = nameof(InternetSentryWorker);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Started {_serviceName}");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await _techniService.RunDiag(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"{_serviceName} is stopping");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{_serviceName} caught exception", ex);
            }
        }
    }
}