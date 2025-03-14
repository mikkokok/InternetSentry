using InternetSentry.Models;
using InternetSentry.Utils;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace InternetSentry.Services.Clients
{
    public sealed class TechniClient
    {
        private readonly string _serviceName = nameof(TechniClient);
        private ILogger<TechniService> _logger;
        private readonly IConfiguration _config;
        private readonly IConfiguration _clientConfig;
        private readonly IConfiguration _testDestinations;
        private readonly string _data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        private readonly int _timeout = 12000;
        private readonly PingOptions _poptions = new(64, true);
        private readonly byte[] _buffer;
        public PingStatus PingStatus { get; private set; }
        private HttpClient? _httpClient;
        private DateTime _timeNow;



        public TechniClient(ILogger<TechniService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _clientConfig = config.GetSection("Techni");
            _testDestinations = config.GetSection("Destinations");
            _buffer = Encoding.ASCII.GetBytes(_data);
        }

        public async Task<bool> CheckInternetStatus()
        {
            _logger.LogInformation($"{_serviceName}:: start checking internet status");
            _timeNow = DateTime.Now;
            bool isConnected = true;
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
                    isConnected = false;
                }
                string pingDestination = _testDestinations.GetSection("ping").Value ?? "";
                if (!string.IsNullOrEmpty(pingDestination))
                {
                    Ping pingSender = new();
                    PingReply PingResult = pingSender.Send(pingDestination, _timeout, _buffer, _poptions);
                    _logger.LogInformation($"{_serviceName}:: Ping result {PingResult.Status} with roundtriptime {PingResult.RoundtripTime} {_timeNow}");
                    PingStatus = new PingStatus { Updated = DateTime.Now, Status = PingResult };

                }
                else
                {
                    _logger.LogWarning($"{_serviceName}:: Ping destination IP not defined in configs ");
                    isConnected = false;
                }
                string wgetDestination = _testDestinations.GetSection("wget").Value ?? "";
                if (!string.IsNullOrEmpty(wgetDestination))
                {
                    var httpClient = GetHttpClient();
                    var response = await httpClient.GetAsync(wgetDestination);
                    _logger.LogInformation($"{_serviceName}:: Wget statuscode {response.StatusCode} {_timeNow}");
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    _logger.LogWarning($"{_serviceName}:: Wget destination not defined in configs ");
                    isConnected = false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{_serviceName}:: Connection check failed {_timeNow}", ex);
                isConnected = false;
            }
            _logger.LogInformation($"{_serviceName}:: Checking internet status resulted in {isConnected} {_timeNow}");
            return isConnected;
        }

        public async Task ForceRestart()
        {
            string cookie = await GetCookie();
            _logger.LogInformation($"{_serviceName}:: Cookie {cookie} {_timeNow}");
            SaltResponse saltContent = await GetLoginSalt(cookie);
            _logger.LogInformation($"{_serviceName}:: Salt {saltContent.ToString()} {_timeNow}");

        }

        private async Task<string> GetCookie()
        {
            string baseUrl = _clientConfig.GetSection("url").Value ?? throw new Exception("Missing URl configuration");
            string cookieUrl = baseUrl + "/api/v1/session/language";
            var httpClient = GetHttpClient();
            var response = await httpClient.GetAsync(cookieUrl);
            _logger.LogInformation($"{_serviceName}:: Cookie fetch statuscode {response.StatusCode} {_timeNow}");
            response.EnsureSuccessStatusCode();
            return response.Headers.GetValues("Set-Cookie").FirstOrDefault() ?? throw new Exception();
        }

        private async Task<SaltResponse> GetLoginSalt(string cookie)
        {
            Uri baseUrl = new(_clientConfig.GetSection("url").Value ?? throw new Exception("Missing URl configuration"));
            Uri saltUrl = new(baseUrl, "/api/v1/session/login");
            string userName = _clientConfig.GetSection("username").Value ?? throw new Exception("Missing username configuration");
            string[] sessid = cookie.Split(";")[0].Split("=");

            var cookieContainer = new CookieContainer();
            var _certificateValidator = new CertificateValidator(_config);
            using var handler = new HttpClientHandler() { CookieContainer = cookieContainer, ServerCertificateCustomValidationCallback = _certificateValidator.ValidateCertificate };
            using var client = new HttpClient(handler) { BaseAddress = saltUrl };
            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("username", userName),
                    new KeyValuePair<string, string>("password", "seeksalthash"),
                ]);
            cookieContainer.Add(saltUrl, new Cookie(sessid[0], sessid[1], "/", baseUrl.Host));
            var response = client.PostAsync("", content).Result;
            _logger.LogInformation($"{_serviceName}:: Salt fetch statuscode {response.StatusCode} {_timeNow}");
            response.EnsureSuccessStatusCode();
            string responststr = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"{_serviceName}:: Return response {responststr} {_timeNow}");
            return JsonSerializer.Deserialize<SaltResponse>(responststr) ?? throw new Exception($"Cannot deserialize {responststr}");
        }

        private HttpClient GetHttpClient()
        {
            if (_httpClient == null)
            {
                var _certificateValidator = new CertificateValidator(_config);
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = _certificateValidator.ValidateCertificate
                };
                _httpClient = new HttpClient(handler)
                {
                    Timeout = new TimeSpan(0, 0, 30)
                };
            }
            return _httpClient;
        }
    }
}
