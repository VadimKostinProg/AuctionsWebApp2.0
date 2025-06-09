using BidMasterOnline.Domain.Models.Entities;

namespace Payments.Service.API.ServiceContracts
{
    public interface INotificationsService
    {
        Task SendMessageOfPerformingPaymentToSeller(Auction auction);
    }
}
