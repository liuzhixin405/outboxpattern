namespace BillAPI
{
    public class BillingRecord
    {
        public int Id { get; set; }
        public Guid OrderEventId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
