using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BidMasterOnline.Application.Initializers
{
    public class UsersInitializer
    {
        private readonly IUserManager _userManager;
        private readonly IRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsersInitializer> _logger;

        public UsersInitializer(IUserManager userManager, IRepository repository, IConfiguration configuration,
            ILogger<UsersInitializer> logger)
        {
            _userManager = userManager;
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
        }

        public void Initialize()
        {
            if (_repository.AnyAsync<User>().GetAwaiter().GetResult())
            {
                return;
            }

            _logger.LogInformation("Starting initializing system administator.");

            var username = _configuration["SysAdmin:Username"]!;
            var fullname = _configuration["SysAdmin:Fullname"]!;
            var email = _configuration["SysAdmin:Email"]!;
            var dateOfBirth = DateOnly.Parse(_configuration["SysAdmin:DateOfBirth"]!);
            var password = _configuration["SysAdmin:Password"]!;

            var sysAdmin = new CreateUserDTO
            {
                Username = username,
                Email = email,
                FullName = fullname,
                DateOfBirth = dateOfBirth,
                Password = password,
                ConfirmPassword = password
            };

            _userManager.CreateUserAsync(sysAdmin, Enums.UserRole.Admin).GetAwaiter().GetResult();

            _logger.LogInformation("System administator has been successfully initialized.");
        }
    }
}
