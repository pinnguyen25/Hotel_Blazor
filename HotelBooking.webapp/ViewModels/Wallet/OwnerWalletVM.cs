
// HotelBooking.Client/ViewModels/OwnerWalletVM.cs
public class OwnerWalletVM
{
    public decimal Balance { get; set; }
    public decimal TotalEarnings { get; set; }
    public List<WalletTransactionVM> RecentTransactions { get; set; } = new();
}

public class WalletTransactionVM
{
    public decimal Amount { get; set; }
    public string Type { get; set; } // BookingRevenue, CommissionFee, Withdrawal...
    public string Description { get; set; }
    public DateTime Date { get; set; }
}

public class WithdrawRequestVM
{
    public decimal Amount { get; set; }
    public string BankName { get; set; } = "";
    public string BankAccountNumber { get; set; } = "";
    public string BankAccountName { get; set; } = "";
}