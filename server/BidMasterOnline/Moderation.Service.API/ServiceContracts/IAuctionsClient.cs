namespace Moderation.Service.API.ServiceContracts
{
    public interface IAuctionsClient
    {
        Task<bool> ApproveAuctionRequestAsync(long auctionRequestId);
        Task<bool> DeclineAuctionRequestAsync(long auctionRequestId, string reason);
        Task<bool> CancelAuctionAsync(long auctionId, string reason);
        Task<bool> RecoverAuctionAsync(long auctionId);
    }
}
