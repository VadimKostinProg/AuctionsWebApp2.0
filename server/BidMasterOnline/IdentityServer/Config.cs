using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services,
            IConfiguration configuration)
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
                .AddTestUsers([
                        new TestUser()
                        {
                            SubjectId = "1",
                            Username = "admin",
                            Password = "admin",
                            Claims =
                            [
                                new Claim(JwtClaimTypes.GivenName, "admin"),
                                new Claim(JwtClaimTypes.Role, "Admin"),
                            ]
                        },
                        new TestUser()
                        {
                            SubjectId = "2",
                            Username = "testModerator",
                            Password = "moderator",
                            Claims =
                            [
                                new Claim(JwtClaimTypes.GivenName, "testModerator"),
                                new Claim(JwtClaimTypes.Role, "Moderator"),
                            ]
                        },
                        new TestUser()
                        {
                            SubjectId = "3",
                            Username = "testParticipant",
                            Password = "participant",
                            Claims =
                            [
                                new Claim(JwtClaimTypes.GivenName, "testParticipant"),
                                new Claim(JwtClaimTypes.Role, "Participant"),
                            ]
                        }
                    ])
                .AddDeveloperSigningCredential();

            return services;
        }
    }
}
