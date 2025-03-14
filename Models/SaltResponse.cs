namespace InternetSentry.Models
{
    public sealed class SaltResponse
    {
        public required string error { get; set; }
        public required string salt { get; set; }
        public required string saltwebui { get; set; }
    }
}