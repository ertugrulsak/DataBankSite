using System;
using System.Collections.Generic;

namespace InternProject.Models;

public class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool UserStatus { get; set; }
}
