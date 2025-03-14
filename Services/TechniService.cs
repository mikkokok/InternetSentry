
using InternetSentry.Models;
using InternetSentry.Services.Clients;
using System.Net.NetworkInformation;

namespace InternetSentry.Services
{
    public sealed class TechniService(TechniClient techniClient, ILogger<TechniService> logger)
    {
        private readonly TechniClient _client = techniClient;
        private readonly ILogger<TechniService> _logger = logger;
        public CurrentStatus CurrenStatus { get; private set; } = new CurrentStatus { Status = ConnStatus.Connected, Updated = DateTime.Now };
        public List<PingStatus> Pings { get; private set; } = [];

        public async Task RunDiag(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Started {GetType().Name}");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    bool isConnected = await _client.CheckInternetStatus();
                    if (_client.PingStatus.Status.Status == IPStatus.Success)
                    {
                        Pings.Add(_client.PingStatus);
                    }
                    if (isConnected)
                    {
                        await _client.ForceRestart();
                    }
                    if (DateTime.Now.Day == 28 && DateTime.Now.Hour == 23)
                    {
                        Pings.Clear();
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