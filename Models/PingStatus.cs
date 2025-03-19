using System.Net.NetworkInformation;

namespace InternetSentry.Models
{
    public class PingStatus
    {
        public IPStatus Status { get; set; }
        public DateTime Updated { get; set; }
        public long RoundTripTime { get; set; }
    }
}
