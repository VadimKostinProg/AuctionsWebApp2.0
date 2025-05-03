using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
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
                            ClientId = "testParticipant",
                            AllowedGrantTypes = GrantTypes.ClientCredentials,
                            ClientSecrets = [new Secret("secret".Sha256())],
                            AllowedScopes = ["participantScope"]
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
