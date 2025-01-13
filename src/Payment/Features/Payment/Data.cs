namespace Payment.Features.Payment
{
    internal sealed class Payment
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
    }
}
