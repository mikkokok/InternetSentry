
using InternetSentry.Services.Clients;

namespace InternetSentry.Services
{
    public sealed class TechniService(TechniClient techniClient, ILogger<TechniService> logger) : BackgroundService
    {
        private readonly TechniClient _client = techniClient;
        private readonly ILogger<TechniService> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Started {GetType().Name}");

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(30));

            try
            {
                while(await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await _client.CheckInternetStatus();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"{GetType().Name} is stopping");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{GetType().Name} caught exception", ex);
            }
        }


    }
}
