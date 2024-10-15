﻿using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Db;

public partial class ProductGalery
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string? ImageName { get; set; }

    public virtual Product Product { get; set; } = null!;
}
