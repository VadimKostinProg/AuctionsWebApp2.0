using BidMasterOnline.Core.DTO;

namespace BidMasterOnline.Core.ServiceContracts
{
    public interface INotificationsQueueProducer
    {
        Task PushNotificationAsync(EmailNotificationDTO notification);
    }
}
