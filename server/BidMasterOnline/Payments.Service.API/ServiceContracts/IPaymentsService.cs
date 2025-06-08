using BidMasterOnline.Core.DTO;
using Payments.Service.API.DTO;

namespace Payments.Service.API.ServiceContracts
{
    public interface IPaymentsService
    {
        Task<ServiceResult<string>> CreateSetupIntentAsync();
        Task<ServiceResult> ConfirmSetupIntentAsync(SetupIntentConfirmRequest request);
        Task<ServiceResult> ChargeAuctionWin(AuctionPaymentRequest paymentRequest);
    }
}
