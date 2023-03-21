namespace TradingAPI
{
    public class CreateOrderDTO
    {
        public int CustomerId { get; set; }
        public int Number { get;  set; }
        public decimal Price { get;  set; }
        public OrderStatus Status { get;  set; }
    }
}
