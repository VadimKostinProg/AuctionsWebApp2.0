using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityServer.Services;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityServer()
                .AddInMemoryClients([
                        new Client
                        {
                            ClientId = "ParticipantUI",
                            AllowedGrantTypes = GrantTypes.Code,
                            RequirePkce = true,
                            RequireClientSecret = false,
                            AllowedScopes = ["openid", "profile", "participantScope"],
                            RedirectUris = { "http://localhost:4200/auth/callback" },
                            PostLogoutRedirectUris = { "http://localhost:4200/" },
                            AllowedCorsOrigins = { "http://localhost:4200" },
                        },
                        new Client
                        {
                            ClientId = "ModeratorUI",
                            AllowedGrantTypes = GrantTypes.Code,
                            RequirePkce = true,
                            RequireClientSecret = false,
                            AllowedScopes = ["openid", "profile", "moderatorScope"],
                            RedirectUris = { "http://localhost:4201/auth/callback" },
                            PostLogoutRedirectUris = { "http://localhost:4201/" },
                            AllowedCorsOrigins = { "http://localhost:4201" },
                        },
                        new Client
                        {
                            ClientId = "Postman",
                            AllowedGrantTypes = GrantTypes.Code,
                            RequirePkce = true,
                            RequireClientSecret = false,
                            AllowedScopes = ["openid", "profile", "participantScope", "moderatorScope"],
                            RedirectUris = { "https://oauth.pstmn.io/v1/callback" }
                        },
                        new Client
                        {
                            ClientId = "auctions-service-api-swagger",
                            ClientName = "Auctions API - Swagger",
                            AllowedGrantTypes = GrantTypes.Code,
                            RequirePkce = true,
                            RequireClientSecret = false,
                            RedirectUris = [$"{configuration["APIResources:AuctionsServiceAPI"]}/swagger/oauth2-redirect.html"],
                            AllowedCorsOrigins = [configuration["APIResources:AuctionsServiceAPI"]!],
                            AllowedScopes = ["openid", "profile", "participantScope", "moderatorScope"]
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
