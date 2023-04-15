using System;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace VillaIdentity;

public static class StaticDetails
{
    public const string Admin = "admin";
    public const string Customer = "customer";


    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
            {
                new ApiScope("villa", "Villa Server"),
                new ApiScope(name: "read",   displayName: "Read your data."),
                new ApiScope(name: "write",  displayName: "Write your data."),
                new ApiScope(name: "delete", displayName: "Delete your data.")
            };

    public static IEnumerable<Client> Cleints =>
         new List<Client>
            {
                new Client
                {
                    ClientId = "service.client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1", "api2.read_only" }
                },
                new Client
                {
                    ClientId = "villa",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes = { "villa",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        JwtClaimTypes.Role
                    },
                    RedirectUris={ "https://localhost:7096/signin-oidc" },
                    PostLogoutRedirectUris={"https://localhost:7096/signout-callback-oidc"},
                }
            };  
}

