using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for sending messages to users.
    /// </summary>
    public interface INotificationsService
    {
        /// <summary>
        /// Sends the message to user of blocking his account.
        /// </summary>
        /// <param name="user">User to send the message to.</param>
        /// <param name="blockingReason">The reason of blocking.</param>
        /// <param name="days">Amount of days of blocking.</param>
        void SendMessageOfBlockingAccountToUser(User user, string blockingReason, int? days);

        /// <summary>
        /// Sends the message to user of unblocking his account.
        /// </summary>
        /// <param name="user">User to send the message to.</param>
        void SendMessageOfUnblockingAccountToUser(User user);

        /// <summary>
        /// Sends the message to user of deleting his account.
        /// </summary>
        /// <param name="user">User to send the message to.</param>
        void SendMessageOfDeletingAccountToUser(User user);

        /// <summary>
        /// Sends the message to user to confirm his email.
        /// </summary>
        /// <param name="user">User to send the message to.</param>
        void SendMessageOfEmailConfirmToUser(User user);

        /// <summary>
        /// Sends the message to user of successfull email confirmation.
        /// </summary>
        /// <param name="user">User to send the message to.</param>
        void SendMessageOfSuccessfullConfirmingEmailToUser(User user);

        /// <summary>
        /// Sends the message to auctionist of publishing his auction.
        /// </summary>
        /// <param name="auction">Published auction.</param>
        void SendMessageOfPublishingAuctionToAuctionst(Auction auction);

        /// <summary>
        /// Sends the message to auctionist of approval his auction.
        /// </summary>
        /// <param name="auction">Approved auction.</param>
        void SendMessageOfApprovalAuctionToAuctionist(Auction auction);

        /// <summary>
        /// Sends the message to auctionist of rejection his auction.
        /// </summary>
        /// <param name="auction">Rejected auction.</param>
        /// <param name="rejectionReason">Rejection reason.</param>
        void SendMessageOfRejectionAuctionToAuctionist(Auction auction, string rejectionReason);

        /// <summary>
        /// Sends the message to auctionist of canceling his auction.
        /// </summary>
        /// <param name="auction">Canceled auction.</param>
        void SendMessageOfCancelingAuctionToAuctionist(Auction auction, string cancelingReason);

        /// <summary>
        /// Sends the message to auctionist of recovering his auction.
        /// </summary>
        /// <param name="auction">Recovered auction.</param>
        void SendMessageOfRecoveringAuctionToAuctionist(Auction auction);

        /// <summary>
        /// Sends the message to the winner of canceling his bid.
        /// </summary>
        /// <param name="bid">Canceled bid.</param>
        void SendMessageOfCancelingTheBidToWinner(Bid bid);

        /// <summary>
        /// Sends the message to auctionist to set payment account for winner to pay for auctions lot.
        /// </summary>
        /// <param name="auction">Finished auction.</param>
        void SendMessageOfPaymentOptionsSetToAuctionist(Auction auction);

        /// <summary>
        /// Sends the message to winner to set delivery information for auctionist to apply delivery 
        /// of the auctions lot.
        /// </summary>
        /// <param name="auction">Finished auction.</param>
        /// <param name="winner">Winner of the auction.</param>
        void SendMessageOfDeliveryOptionsSetToWinner(Auction auction, User winner);

        /// <summary>
        /// Sends the message to winner to apply payment of auctions lot.
        /// </summary>
        /// <param name="auction">Finished auction.</param>
        /// <param name="winner">Winner of the auction.</param>
        void SendMessageOfApplyingPaymentToWinner(Auction auction, User winner);

        /// <summary>
        /// Sends the message to auctionist to apply delivery of auctions lot.
        /// </summary>
        /// <param name="auction">Finished auction.</param>
        void SendMessageOfApplyingDeliveryToAuctionist(Auction auction);

        /// <summary>
        /// Sends the message to winner of confirmation of delivery of auctions lot.
        /// </summary>
        /// <param name="auction">Finished auction.</param>
        /// <param name="winner">Winner of the auction.</param>
        void SendMessageOfDeliveryConfirmationToWinner(Auction auction, User winner);

        /// <summary>
        /// Sends the message to auctionist of confirmation of payment for auctions lot.
        /// </summary>
        /// <param name="auction">Finished auction.</param>
        /// <param name="winner">Winner of the auction.</param>
        void SendMessageOfPaymentConfirmationToAuctionist(Auction auction);

        /// <summary>
        /// Sends the message to auctionist of finishing his auction wis no winner.
        /// </summary>
        /// <param name="auction">Finished auction.</param>
        void SendMessageOfNoWinnersOfAuctionToAuctionist(Auction auction);
    }
}