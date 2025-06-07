using BidMasterOnline.Domain.Models.Entities;

namespace Auctions.Service.API.ServiceContracts
{
    public interface INotificationsService
    {
        Task SendMessageOfApprovalAuctionRequestToAuctioneer(AuctionRequest auction);
        Task SendMessageOfCancelingAuctionToAuctioneer(Auction auction);
        Task SendMessageOfDeletingAccountToUser(User recepient);
        Task SendMessageOfNoWinnersOfAuctionToAuctioneer(Auction auction);
        Task SendMessageOfPublishingAuctionRequestToUserAsync(AuctionRequest auctionRequest);
        Task SendMessageOfRecoveringAuctionToAuctioneer(Auction auction);
        Task SendMessageOfDecliningAuctionRequestToUser(AuctionRequest auctionRequest);
    }
}
