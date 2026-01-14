using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class WithdrawalRequest
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public int WalletId { get; set; }

    public decimal Amount { get; set; }

    public string BankName { get; set; } = null!;

    public string BankAccountNumber { get; set; } = null!;

    public string BankAccountName { get; set; } = null!;

    public string? Status { get; set; }

    public string? AdminNote { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public int? ApprovedBy { get; set; }

    public virtual User Owner { get; set; } = null!;

    public virtual OwnerWallet Wallet { get; set; } = null!;
}
