﻿using System;
using System.Collections.Generic;

namespace CloneEbay.Data.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? UserId { get; set; }

    public decimal? Amount { get; set; }

    public string? Method { get; set; }

    public string? Status { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? TransactionId { get; set; }

    public virtual OrderTable? Order { get; set; }

    public virtual User? User { get; set; }
}
