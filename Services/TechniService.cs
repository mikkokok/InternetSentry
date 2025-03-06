
using InternetSentry.Models;
using InternetSentry.Services.Clients;

namespace InternetSentry.Services
{
    public sealed class TechniService(TechniClient techniClient, ILogger<TechniService> logger)
    {
        private readonly TechniClient _client = techniClient;
        private readonly ILogger<TechniService> _logger = logger;
        public CurrentStatus CurrenStatus { get; private set; } = new CurrentStatus { Status = ConnStatus.Connected, Updated = DateTime.Now };

        public async Task RunDiag(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Started {GetType().Name}");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    bool status = await _client.CheckInternetStatus();
                    if (!status)
                    {

                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
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