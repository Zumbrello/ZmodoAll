using System;
using System.Collections.Generic;

namespace GamesLibAPI.Models;

public partial class UserMobileAccount
{
    public int Id { get; set; }

    public string Login { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    
}
