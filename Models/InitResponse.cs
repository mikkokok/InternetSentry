namespace InternetSentry.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class InitResponse
    {
        public required string error { get; set; }
        public required string message { get; set; }
        public required List<object> data { get; set; }
        public required string token { get; set; }
    }
}