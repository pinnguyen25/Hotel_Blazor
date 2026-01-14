public class PaymentRequestDTO
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Banking"; // Banking, Cash, VNPAY
    public string TransactionId { get; set; } // Mã giao dịch ngân hàng
}

public class PaymentResponseDTO
{
    public bool Success { get; set; }
    public string PaymentMethod { get; set; }
    public string OrderDescription { get; set; }
    public string OrderId { get; set; }
    public string PaymentId { get; set; }
    public string TransactionId { get; set; }
    public string Token { get; set; }
    public string VnPayResponseCode { get; set; }
}

public class CreatePaymentReq {
    public int BookingId { get; set; }
}