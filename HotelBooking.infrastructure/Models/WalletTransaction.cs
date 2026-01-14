using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class WalletTransaction
{
    public int Id { get; set; }

    public int WalletId { get; set; }

    public decimal Amount { get; set; }

    public string TransactionType { get; set; } = null!;

    public string? Description { get; set; }

    public int? ReferenceId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual OwnerWallet Wallet { get; set; } = null!;
}
