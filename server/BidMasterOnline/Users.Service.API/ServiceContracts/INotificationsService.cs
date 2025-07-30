using BidMasterOnline.Domain.Models.Entities;

namespace Users.Service.API.ServiceContracts
{
    public interface INotificationsService
    {
        Task SendMessageOfBlockingUserAsync(User user);
        Task SendMessageOfUnblockingUserAsync(User user);
        Task SendMessageOfDeletingAccountToUser(User recepient);
    }
}
