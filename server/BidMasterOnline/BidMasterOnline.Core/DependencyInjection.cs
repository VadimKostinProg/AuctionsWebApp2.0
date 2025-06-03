using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BidMasterOnline.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IImagesService, ImagesService>();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IUserStatusValidationService, UserStatusValidationService>();

            return services;
        }
    }
}
