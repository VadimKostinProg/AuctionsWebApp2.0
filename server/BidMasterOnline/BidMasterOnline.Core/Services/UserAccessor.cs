using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace BidMasterOnline.Core.Services
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository _repository;

        public UserAccessor(IHttpContextAccessor httpContextAccessor, IRepository repository)
        {
            _httpContextAccessor = httpContextAccessor;

            InitUserId();
            _repository = repository;
        }

        private long _userId;
        private User? _user;

        public long UserId => _userId;

        public string UserName
        {
            get
            {
                this.EnsureUserInitialized();

                return _user!.Username;
            }
        }

        public string Email
        {
            get
            {
                this.EnsureUserInitialized();

                return _user!.Email;
            }
        }

        public string Role
        {
            get
            {
                this.EnsureUserInitialized();

                return _user!.Role!.Name;
            }
        }

        private void InitUserId()
        {
            // TODO: add custom exception
            string userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value
                ?? throw new ArgumentNullException();

            _userId = long.Parse(userId);
        }

        private void EnsureUserInitialized()
        {
            if (_user == null)
            {
                _user = _repository.GetById<User>(_userId);
            }
        }
    }
}
