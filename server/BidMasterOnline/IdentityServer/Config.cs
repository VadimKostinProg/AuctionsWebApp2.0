using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityServer.Services;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddInMemoryClients([
                        new Client
                        {
                            ClientId = "ParticipantUI",
                            AllowedGrantTypes = GrantTypes.Code,
                            ClientSecrets = [new Secret("secret".Sha256())],
                            AllowedScopes = ["participantScope"],
                            RequirePkce = true,
                            RedirectUris = { "http://localhost:4200/callback" },
                            PostLogoutRedirectUris = { "http://localhost:4200/" },
                            AllowedCorsOrigins = { "http://localhost:4200" },
                        },
                        new Client
                        {
                            ClientId = "ModeratorUI",
                            AllowedGrantTypes = GrantTypes.Code,
                            ClientSecrets = [new Secret("secret".Sha256())],
                            AllowedScopes = ["moderatorScope"],
                            RequirePkce = true,
                            RedirectUris = { "http://localhost:4201/callback" },
                            PostLogoutRedirectUris = { "http://localhost:4201/" },
                            AllowedCorsOrigins = { "http://localhost:4201" },
                        },
                        new Client
                        {
                            ClientId = "Postman",
                            AllowedGrantTypes = GrantTypes.Code,
                            ClientSecrets = [new Secret("postman_secret".Sha256())],
                            AllowedScopes = ["openid", "profile", "participantScope", "moderatorScope"],
                            RequirePkce = true,
                            RedirectUris = { "https://oauth.pstmn.io/v1/callback" }
                        }
                    ])
                .AddInMemoryApiResources([
                        new ApiResource()
                        {
                            Name = "Auctions.Service.API",
                            Scopes = ["participantScope", "moderatorScope"],
                        },
                        new ApiResource()
                        {
                            Name = "Bids.Service.API",
                            Scopes = ["participantScope", "moderatorScope"],
                        },
                        new ApiResource()
                        {
                            Name = "Feedbacks.Service.API",
                            Scopes = ["participantScope", "moderatorScope"],
                        },
                        new ApiResource()
                        {
                            Name = "Moderation.Service.API",
                            Scopes = ["moderatorScope"],
                        },
                        new ApiResource()
                        {
                            Name = "Payments.Service.API",
                            Scopes = ["participantScope", "moderatorScope"],
                        },
                        new ApiResource()
                        {
                            Name = "Deliveries.Service.API",
                            Scopes = ["participantScope", "moderatorScope"],
                        },
                    ])
                .AddInMemoryApiScopes([
                        new ApiScope("participantScope", "Participant Scope"),
                        new ApiScope("moderatorScope", "Moderator Scope")
                    ])
                .AddInMemoryIdentityResources([
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile()
                    ])
                .AddProfileService<UserProfileService>()
                .AddDeveloperSigningCredential();

            return services;
        }
    }
}
