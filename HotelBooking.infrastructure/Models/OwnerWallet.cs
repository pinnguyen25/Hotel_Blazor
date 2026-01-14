using System;
using System.Collections.Generic;

namespace HotelBooking.infrastructure.Models;

public partial class OwnerWallet
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public decimal Balance { get; set; }

    public decimal TotalEarnings { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();

    public virtual ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = new List<WithdrawalRequest>();
}
