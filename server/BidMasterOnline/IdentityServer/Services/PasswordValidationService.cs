using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Models.Entities;
using Duende.IdentityModel;
using IdentityServer.Helpers;
using IdentityServer.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityServer.Services
{
    public class PasswordValidationService : IPasswordValidationService
    {
        private readonly IRepository _repository;

        public PasswordValidationService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> ValidateAsync(string username, string password)
        {
            User? user = await _repository.GetFirstOrDefaultAsync<User>(
                u => u.Email == username || u.Username == username,
                includeQuery: query => query.Include(e => e.Role)!);

            if (user != null && ValidatePassword(user, password))
            {
                return user;
            }

            return null;
        }

        private bool ValidatePassword(User user, string password)
            => user.PasswordHashed == CryptographyHelper.Hash(password, user.PasswordSalt); 
        
        private static List<Claim> GetUserClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(JwtClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtClaimTypes.Role, user.Role?.Name ?? "User")
            };
        }
    }
}
