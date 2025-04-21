using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Helpers;
using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Sessions;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace BidMasterOnline.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly SessionContext _sessionContext;

        public AuthService(IRepository repository, IConfiguration configuration, SessionContext sessionContext)
        {
            _repository = repository;
            _configuration = configuration;
            _sessionContext = sessionContext;
        }

        public async Task<UserDTO> GetAuthenticatedUserAsync()
        {
            var user = await this.GetAuthenticatedUserEntityAsync();

            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name,
                ImageUrl = user.ImageUrl,
                Status = user.UserStatus.Name
            };
        }

        public async Task<User> GetAuthenticatedUserEntityAsync()
        {
            if (_sessionContext.UserId is null)
                throw new AuthenticationException("User is not authenticated.");

            var user = await _repository.FirstOrDefaultAsync<User>(x => x.Id == _sessionContext.UserId &&
                                         x.UserStatus.Name != Enums.UserStatus.Deleted.ToString());

            if (user is null)
                throw new UnauthorizedAccessException("User with such claims does not exist.");

            return user;
        }

        public async Task<AuthenticationDTO> LoginAsync(LoginDTO login)
        {
            if (login is null)
                throw new ArgumentNullException("Login is null.");

            // Find user by passed credentials
            var user = await _repository.FirstOrDefaultAsync<User>(x =>
                (x.Username == login.UserName || x.Email == login.UserName) && 
                x.UserStatus.Name != Enums.UserStatus.Deleted.ToString(),
                disableTracking: false);

            if (user is null)
                throw new KeyNotFoundException("User with such user name or email does not exist.");

            // Validating password
            var hashedInputPassword = CryptographyHelper.Hash(login.Password, user.PasswordSalt);

            if (!user.PasswordHashed.Equals(hashedInputPassword))
                throw new ArgumentException("Password is incorrect.");

            // Generate token
            var token = this.GenerateJWT(user);

            return new AuthenticationDTO
            {
                UserId = user.Id,
                Role = user.Role.Name,
                Status = user.UserStatus.Name,
                Token = token
            };
        }

        private string GenerateJWT(User user)
        {
            // Get token expiration 
            var expiration = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            // Configure claims for the payload
            var claims = new Claim[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var securityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var signinCredentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenGenerator =
                new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: expiration,
                    signingCredentials: signinCredentials
                );

            var tokenHandler =
                new JwtSecurityTokenHandler();

            var token = tokenHandler.WriteToken(tokenGenerator);

            return token;
        }
    }
}
