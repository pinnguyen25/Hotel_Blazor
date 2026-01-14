public class PromotionCreateVM
{
    public string Code { get; set; }
    public string Name { get; set; }
    public int DiscountType { get; set; } // 1: %, 2: Cash
    public decimal DiscountValue { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public decimal? MinBookingValue { get; set; }
    public int PromotionCategory { get; set; } // 0: Normal, 1: Owner, 2: Staff, 3: NewUser
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int UsageLimit { get; set; }
    public int UserUsageLimit { get; set; }
    public bool IsActive { get; set; }
}

public class PromotionVM : PromotionCreateVM
{
    public int Id { get; set; }
    public int UsedCount { get; set; }
}