namespace TradingAPI
{
    public class OutBoxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public DateTime OccurredOnUtc { get; set; }
        public DateTime? ProceddedOnUtc { get; set; }
        public string? Error { get; set; }
    }
}
