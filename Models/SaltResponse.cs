namespace InternetSentry.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public sealed class SaltResponse
    {
        public required string error { get; set; }
        public required string salt { get; set; }
        public required string saltwebui { get; set; }
    }
}