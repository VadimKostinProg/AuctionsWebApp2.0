using BidMasterOnline.Application.Sessions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BidMasterOnline.API.Filters
{
    public sealed class SessionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var services = context.HttpContext.RequestServices;
            var session = services.GetService(typeof(SessionContext)) as SessionContext;
            var claimsIdentity = context.HttpContext.User.Identity as ClaimsIdentity;

            if (claimsIdentity is not null)
            {
                var userIdClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == "Id");
                if (userIdClaim is not null)
                {
                    session.UserId = Guid.Parse(userIdClaim.Value);
                }

                var roleClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role);
                if (roleClaim is not null)
                {
                    session.UserRole = roleClaim.Value;
                }
            }

            await next();
        }
    }
}
