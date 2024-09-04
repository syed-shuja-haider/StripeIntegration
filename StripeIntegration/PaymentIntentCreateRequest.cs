namespace StripeIntegration
{
    public class PaymentIntentCreateRequest
    {
        public long Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
    }
}
