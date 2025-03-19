namespace InternetSentry.Models
{
    public class LoggedResponse
    {
        public required string error { get; set; }
        public required string message { get; set; }
        public required Data data { get; set; }
    }

    public class Data
    {
        public required int failedAttempts { get; set; }
    }
}
