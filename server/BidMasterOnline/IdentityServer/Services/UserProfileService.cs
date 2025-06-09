using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Models.Entities;
using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityServer.Services
{
    public class UserProfileService : IProfileService
    {
        private readonly IRepository _repository;

        public UserProfileService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            long userId = long.Parse(context.Subject.GetSubjectId());

            User? user = await _repository.GetFirstOrDefaultAsync<User>(e => e.Id == userId,
                includeQuery: query => query.Include(e => e.Role)!);

            if (user != null)
            {
                context.IssuedClaims = GetUserClaims(user);
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            long userId = long.Parse(context.Subject.GetSubjectId());
            User? user = await _repository.GetFirstOrDefaultAsync<User>(u => u.Id == userId && !u.Deleted, disableTracking: true);
            context.IsActive = user != null;
        }

        private static List<Claim> GetUserClaims(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(JwtClaimTypes.PreferredUserName, user.Username ?? string.Empty),
                new Claim(JwtClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtClaimTypes.Role, user.Role!.Name),
                new Claim("user_status", user.Status.ToString())
            };

            return claims;
        }
    }
}
