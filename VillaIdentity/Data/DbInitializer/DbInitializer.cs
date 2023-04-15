using System;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VillaIdentity.Models;

namespace VillaIdentity.Data.DbInitializer;

public class DbInitializer : IDbInitializer
{

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public void Initialize()
    {
        if (_roleManager.FindByNameAsync(StaticDetails.Admin).Result == null)
        {
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.Customer)).GetAwaiter().GetResult();
        }
        else return;

        ApplicationUser adminUser = new()
        {
            UserName = "admin1@gmail.com",
            Email = "admin1@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "111111111111",
            Name = "Alex Admin",
        };

        _userManager.CreateAsync(adminUser, "Admin123!").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(adminUser, StaticDetails.Admin).GetAwaiter().GetResult();

        IdentityResult claims1 = _userManager.AddClaimsAsync(adminUser, new Claim[] {
                new Claim(JwtClaimTypes.Name,adminUser.Name),
                new Claim(JwtClaimTypes.Role,StaticDetails.Admin)
            }).Result;



        ApplicationUser customerUser = new()
        {
            UserName = "customer1@gmail.com",
            Email = "customer1@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "111111111111",
            Name = "Alex Customer",
        };

        _userManager.CreateAsync(customerUser, "Customer123!").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(customerUser, StaticDetails.Customer).GetAwaiter().GetResult();

        IdentityResult temp2 = _userManager.AddClaimsAsync(customerUser, new Claim[] {
                 new Claim(JwtClaimTypes.Name,customerUser.Name),
                new Claim(JwtClaimTypes.Role,StaticDetails.Customer),
            }).Result;
    }
}

