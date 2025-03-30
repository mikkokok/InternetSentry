
using InternetSentry.Models;
using InternetSentry.Services.Clients;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace InternetSentry.Services
{
    public sealed class TechniService(TechniClient techniClient, RozalinaClient rozalinaClient, ILogger<TechniService> logger)
    {
        private readonly TechniClient _client = techniClient;
        private readonly RozalinaClient _rozalinaClient = rozalinaClient;
        private readonly ILogger<TechniService> _logger = logger;
        public CurrentStatus CurrentStatus { get; private set; } = new CurrentStatus { Status = ConnStatus.Connected, Updated = DateTime.Now };
        public List<PingStatus> Pings { get; private set; } = [];

        public async Task RunDiag(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Started {GetType().Name}");
            int pingCount = 29;
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    bool sendRestart = false;
                    bool isConnected = await _client.CheckInternetStatus();
                    if (_client.PingStatus.Status == IPStatus.Success)
                    {
                        pingCount++;
                        if (pingCount == 30)
                        {
                            Pings.Add(_client.PingStatus);
                            pingCount = 0;
                        }
                    }
                    if (!isConnected)
                    {
                        try
                        {
                            sendRestart = true;
                            await _client.ForceRestart();
                            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                        }
                        catch (JsonException jex)
                        {
                            _logger.LogInformation($"{GetType().Name} something happened but we should try again", jex);
                            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
                        }
                    }
                    if (isConnected && sendRestart)
                    {
                        await _rozalinaClient.SendTelegramMessage("InternetSentry", false, "Cable modem rebooted");
                        sendRestart = false;
                    }
                    if (DateTime.Now.Day % 7 == 0 && DateTime.Now.Hour == 23)
                    {
                        Pings.Clear();
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    UpdateStatus(isConnected);
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
            UpdateStatus(false);
        }
        private void UpdateStatus(bool connected)
        {
            if (connected)
            {
                CurrentStatus = new CurrentStatus { Status = ConnStatus.Connected, Updated= DateTime.Now };
                return;
            }
            CurrentStatus = new CurrentStatus { Status = ConnStatus.Disconnected, Updated = DateTime.Now };
        }
    }
}