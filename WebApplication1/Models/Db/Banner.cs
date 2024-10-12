using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Db;

public partial class Banner
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? SubTitlle { get; set; }

    public string? ImageName { get; set; }

    public short? Priority { get; set; }

    public string? Link { get; set; }

    public string? Position { get; set; }
}
