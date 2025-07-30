using BidMasterOnline.Application.Initializers;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Services;
using BidMasterOnline.Application.Sessions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BidMasterOnline.Application
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserManager, UserManager>();

            services.AddScoped<UsersInitializer>();
            
            services.AddScoped<SessionContext>();

            return services;
        }
    }
}
