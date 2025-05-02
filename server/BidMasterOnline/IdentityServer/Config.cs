using IdentityServer4.Models;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<Client> Clients
            => [];

        public static IEnumerable<ApiResource> ApiResources
            => [];

        public static IEnumerable<ApiScope> ApiScopes
            => [];

        public static IEnumerable<IdentityResource> IdentityResources
            => [];
    }
}
