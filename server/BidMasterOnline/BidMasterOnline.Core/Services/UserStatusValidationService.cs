using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;

namespace BidMasterOnline.Core.Services
{
    public class UserStatusValidationService : IUserStatusValidationService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;

        public UserStatusValidationService(IRepository repository, IUserAccessor userAccessor)
        {
            _repository = repository;
            _userAccessor = userAccessor;
        }

        public Task<bool> IsActiveAsync()
            => _repository.AnyAsync<User>(e => e.Id == _userAccessor.UserId && e.Status == UserStatus.Active);

        public Task<bool> IsPaymentMethodAttachedAsync()
            => _repository.AnyAsync<User>(e => e.Id == _userAccessor.UserId && e.IsPaymentMethodAttached);

        public Task<bool> IsAbleToParticipateInTrades()
            => _repository.AnyAsync<User>(e => e.Id == _userAccessor.UserId && e.Status == UserStatus.Active && e.IsPaymentMethodAttached);

        public Task<bool> IsInStatusAsync(UserStatus status)
            => _repository.AnyAsync<User>(e => e.Id == _userAccessor.UserId && e.Status == status);
    }
}
