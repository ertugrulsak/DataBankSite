using System;
using System.Collections.Generic;

namespace InternProject.Models;

public partial class Connection
{
    public int DbId { get; set; }

    public string DbName { get; set; } = null!;

    public string? DbUserName { get; set; }

    public string? DbPassword { get; set; }

    public string DbLocation { get; set; } = null!;

    public bool DbStatus { get; set; }

    public bool? DbSec { get; set; }
}
