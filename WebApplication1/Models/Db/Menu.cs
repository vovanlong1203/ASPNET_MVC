using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Db;

public partial class Menu
{
    public int Id { get; set; }

    public string? MenuTite { get; set; }

    public string? Link { get; set; }

    public string? Type { get; set; }
}
