namespace InternetSentry.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class LoggedResponse
    {
        public required string error { get; set; }
        public required string message { get; set; }
        public required Data data { get; set; }
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]

    public class Data
    {
        public required int failedAttempts { get; set; }
    }
}
