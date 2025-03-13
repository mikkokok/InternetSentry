using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace InternetSentry.Services.Clients
{
    public sealed class TechniClient
    {
        private readonly string _serviceName = nameof(TechniClient);
        private ILogger<TechniService> _logger;
        private readonly IConfiguration _clientConfig;
        private readonly IConfiguration _testDestinations;
        private readonly string _data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        private readonly int _timeout = 12000;
        private readonly PingOptions _poptions = new PingOptions(64, true);
        private readonly byte[] _buffer;
        public PingReply PingResult { get; private set; }
        private HttpClient? _httpClient;



        public TechniClient(ILogger<TechniService> logger, IConfiguration config)
        {
            _logger = logger;
            _clientConfig = config.GetSection("Techni");
            _testDestinations = config.GetSection("Destinations");
            _buffer = Encoding.ASCII.GetBytes(_data);
        }

        public async Task<bool> CheckInternetStatus()
        {
            _logger.LogInformation($"{_serviceName}:: start checking internet status");
            DateTime dateTime = DateTime.Now;
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
                string pingDestination = _testDestinations.GetSection("ping").Value ?? "";
                if (!string.IsNullOrEmpty(dnsName))
                {
                    Ping pingSender = new();
                    PingReply PingResult = pingSender.Send(pingDestination, _timeout, _buffer, _poptions);
                    _logger.LogInformation($"{_serviceName}:: Ping result {PingResult.Status} with roundtriptime {PingResult.RoundtripTime} {dateTime}");
                }
                else
                {
                    _logger.LogWarning($"{_serviceName}:: Ping destination IP not defined in configs ");
                    didFail = true;
                }
                string wgetDestination = _testDestinations.GetSection("wget").Value ?? "";
                if (!string.IsNullOrEmpty(dnsName))
                {
                    var httpClient = GetHttpClient();
                    var response = await httpClient.GetAsync(wgetDestination);
                    _logger.LogInformation($"{_serviceName}:: Wget statuscode {response.StatusCode} {dateTime}");
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    _logger.LogWarning($"{_serviceName}:: Wget destination not defined in configs ");
                    didFail = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{_serviceName}:: Connection check failed {dateTime}", ex);
                didFail = true;
            }
            _logger.LogInformation($"{_serviceName}:: Checking internet status resulted in {didFail} {dateTime}");
            return didFail;
        }

        public async Task ForceRestart()
        {


        }

        private async Task GetCookie()
        {

        }

        private HttpClient GetHttpClient()
        {
            _httpClient ??= new HttpClient()
            {
                Timeout = new TimeSpan(0, 0, 30)
            };
            return _httpClient;
        }
    }
}
