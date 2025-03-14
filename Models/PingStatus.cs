using System.Net.NetworkInformation;

namespace InternetSentry.Models
{
    public class PingStatus
    {
        public PingReply Status { get; set; }
        public DateTime Updated { get; set; }
    }
}
