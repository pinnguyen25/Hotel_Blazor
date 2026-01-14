public class OwnerWalletDTO
{
    public decimal Balance { get; set; }
    public decimal TotalEarnings { get; set; }
    public List<WalletTransactionDTO> RecentTransactions { get; set; }
}

public class WalletTransactionDTO 
{
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}

public class WithdrawRequestDTO
{
    public decimal Amount { get; set; }
    public string BankName { get; set; }        // Tên ngân hàng
    public string BankAccountNumber { get; set; } // Số tài khoản
    public string BankAccountName { get; set; }   // Tên chủ tài khoản
}