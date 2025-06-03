using BidMasterOnline.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BidMasterOnline.Core.Services
{
    public class UserAccessor : IUserAccessor
    {
        private readonly HttpContext _httpContext;
        private IEnumerable<Claim> _userClaims = [];

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public long UserId
        {
            get
            {
                EnsureUserClaimsAreInitialized();
                return long.Parse(_userClaims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            }
        }

        public string UserName
        {
            get
            {
                EnsureUserClaimsAreInitialized();
                return _userClaims.First(x => x.Type == "preferred_username").Value;
            }
        }

        public string Email
        {
            get
            {
                EnsureUserClaimsAreInitialized();
                return _userClaims.First(x => x.Type == ClaimTypes.Email).Value;
            }
        }

        public string Role
        {
            get
            {
                EnsureUserClaimsAreInitialized();
                return _userClaims.First(x => x.Type == ClaimTypes.Role).Value;
            }
        }

        public long? TryGetUserId()
        {
            if (!(_httpContext.User.Identity?.IsAuthenticated ?? false))
            {
                return null;
            }

            EnsureUserClaimsAreInitialized();

            string? userIdstr = _userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            return string.IsNullOrEmpty(userIdstr)
                ? null
                : long.Parse(userIdstr);
        }

        public string? TryGetUserName()
        {
            if (!(_httpContext.User.Identity?.IsAuthenticated ?? false))
            {
                return null;
            }

            EnsureUserClaimsAreInitialized();

            return _userClaims.FirstOrDefault(x => x.Type == "preferred_username")?.Value;
        }

        private void EnsureUserClaimsAreInitialized()
        {
            if (_userClaims.Any())
                return;

            if (!(_httpContext.User.Identity?.IsAuthenticated ?? false))
            {
                throw new UnauthorizedAccessException("User is not authorized.");
            }

            _userClaims = _httpContext.User.Claims;
        }
    }
}
