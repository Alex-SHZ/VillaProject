using System;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VillaIdentity.Models;
using Duende.IdentityServer.Extensions;

namespace VillaIdentity;

public class ProfileService : IProfileService
{
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly UserManager<ApplicationUser> _userMgr;
    private readonly RoleManager<IdentityRole> _roleMgr;

    public ProfileService(
        UserManager<ApplicationUser> userMgr,
        RoleManager<IdentityRole> roleMgr,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
    {
        _userMgr = userMgr;
        _roleMgr = roleMgr;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }



    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        string sub = context.Subject.GetSubjectId();
        ApplicationUser user = await _userMgr.FindByIdAsync(sub);

        ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);

        List<Claim> claims = userClaims.Claims.ToList();
        claims = claims.Where(u => context.RequestedClaimTypes.Contains(u.Type)).ToList();
        claims.Add(new Claim(JwtClaimTypes.Name, user.Name));
        if (_userMgr.SupportsUserRole)
        {
            IList<string> roles = await _userMgr.GetRolesAsync(user);
            foreach (var rolename in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, rolename));
            }
        }

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        string sub = context.Subject.GetSubjectId();
        ApplicationUser user = await _userMgr.FindByIdAsync(sub);
        context.IsActive = user != null;
    }
}

