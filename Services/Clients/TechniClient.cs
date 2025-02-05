using System.Net;
using System.Net.Sockets;

namespace InternetSentry.Services.Clients
{
    public sealed class TechniClient(ILogger<TechniService> logger, IConfiguration config)
    {
        private ILogger<TechniService> _logger = logger;
        private IConfiguration _clientConfig = config.GetSection("Techni");
        private IConfiguration _testDestinations = config.GetSection("Destinations");


        public async Task CheckInternetStatus()
        {
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
                    _logger.LogWarning($"{GetType().Name} DNS name not defined in configs ");
                    didFail = true;
                }
            }
            catch (SocketException se)
            {
                _logger.LogWarning($"{GetType().Name} DNS query failed", se);
                didFail = true;
            }


            if (didFail) {
                _logger.LogWarning($", proceed on restarting router");
                await ForceRestart();
            }
        }

        public async Task ForceRestart()
        {

        }

    }
}
