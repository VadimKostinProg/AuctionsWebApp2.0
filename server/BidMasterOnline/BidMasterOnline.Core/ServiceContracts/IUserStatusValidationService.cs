using BidMasterOnline.Domain.Enums;

namespace BidMasterOnline.Core.ServiceContracts
{
    public interface IUserStatusValidationService
    {
        Task<bool> IsActiveAsync();
        Task<bool> IsPaymentMethodAttachedAsync();
        Task<bool> IsAbleToParticipateInTrades();
        Task<bool> IsInStatusAsync(UserStatus status);
    }
}
