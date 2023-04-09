using System;
using Microsoft.AspNetCore.Identity;

namespace VillaAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}

