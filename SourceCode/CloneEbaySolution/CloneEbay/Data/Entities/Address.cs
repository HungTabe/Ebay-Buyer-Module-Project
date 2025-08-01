﻿using System;
using System.Collections.Generic;

namespace CloneEbay.Data.Entities;

public partial class Address
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public bool? IsDefault { get; set; }

    public string? PostalCode { get; set; }

    public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();

    public virtual User? User { get; set; }
}
