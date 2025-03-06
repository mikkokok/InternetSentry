using System.Net;
using System.Net.Sockets;

namespace InternetSentry.Services.Clients
{
    public sealed class TechniClient(ILogger<TechniService> logger, IConfiguration config)
    {
        private readonly string _serviceName = nameof(TechniClient);
        private ILogger<TechniService> _logger = logger;
        private readonly IConfiguration _clientConfig = config.GetSection("Techni");
        private readonly IConfiguration _testDestinations = config.GetSection("Destinations");

        public async Task<bool> CheckInternetStatus()
        {
            _logger.LogInformation($"{_serviceName}:: start checking internet status");
            bool didFail = false;
            try
            {
                string dnsName = _testDestinations.GetSection("DNS").Value ?? "";
                if (!string.IsNullOrEmpty(dnsName))
                {
                    IPHostEntry iPHost = await Dns.GetHostEntryAsync(dnsName);
                }
                else
                {
                    _logger.LogWarning($"{_serviceName}:: DNS name not defined in configs ");
                    didFail = true;
                }
            }
            catch (SocketException se)
            {
                _logger.LogWarning($"{_serviceName}:: DNS query failed", se);
                didFail = true;
            }
            _logger.LogInformation($"{_serviceName}:: Checking internet status resulted in {didFail}");
            return didFail;
        }

        public async Task ForceRestart()
        {


        }

        private async Task GetCookie()
        {

        }

    }
}
