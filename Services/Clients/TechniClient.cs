namespace InternetSentry.Services.Clients
{
    public sealed class TechniClient(ILogger<TechniService> logger, IConfiguration config)
    {
        private ILogger<TechniService> _logger = logger;
        private IConfiguration _clientConfig = config.GetSection("Techni");


        public async Task CheckInternetStatus()
        {

        }

        public async Task ForceRestart()
        {

        }

    }
}
