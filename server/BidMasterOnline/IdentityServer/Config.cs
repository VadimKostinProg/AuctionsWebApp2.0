using IdentityServer4.Models;

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
                .AddInMemoryApiResources([])
                .AddInMemoryApiScopes([
                        new ApiScope("participantScope", "Participant Scope")
                    ])
                .AddInMemoryIdentityResources([])
                .AddDeveloperSigningCredential();

            return services;
        }
    }
}
