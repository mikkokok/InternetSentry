namespace InternetSentry.Models
{
    public class InitResponse
    {
        public required string error { get; set; }
        public required string message { get; set; }
        public required List<object> data { get; set; }
        public required string token { get; set; }
    }
}