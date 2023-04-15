using System;
using Microsoft.AspNetCore.Identity;

namespace VillaIdentity.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}

