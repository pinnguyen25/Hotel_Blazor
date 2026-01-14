public class PaymentRequestVM
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string TransactionId { get; set; } = "";
}