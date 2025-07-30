using BidMasterOnline.Domain.Models.Entities;

namespace Feedbacks.Service.API.ServiceContracts
{
    public interface INotificationsService
    {
        Task SendMessageOfProcessingComplaintAsync(Complaint complaint);
        Task SendMessageOfProcessingSupportTicketAsync(SupportTicket supportTicket);
        Task SendMessageOfCompletingComplaintAsync(Complaint complaint);
        Task SendMessageOfCompletingSupportTicketAsync(SupportTicket supportTicket);
    }
}
