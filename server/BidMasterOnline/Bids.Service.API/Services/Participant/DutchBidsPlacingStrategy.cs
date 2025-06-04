using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;
using Bids.Service.API.ServiceContracts.Participant;

namespace Bids.Service.API.Services.Participant
{
    public class DutchBidsPlacingStrategy : IBidsPlacingStrategy
    {
        public ServiceResult PlaceNewBid(Bid newBid, Auction auction)
        {
            ServiceResult result = new();

            if (auction.Status != AuctionStatus.Active || auction.FinishTime <= DateTime.UtcNow)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add($"Could not place bid on inactive auction.");

                return result;
            }

            if (auction.Bids!.Any(e => !e.Deleted) && auction.Bids!
                                                             .Where(e => !e.Deleted)
                                                             .OrderByDescending(x => x.CreatedAt)
                                                             .First()
                                                             .BidderId == newBid.BidderId)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add($"Could not place two bids one after another.");

                return result;
            }

            decimal maxAmount = auction.Bids!.Any(e => !e.Deleted)
                ? auction.Bids!.Where(e => !e.Deleted).Min(e => e.Amount) - auction.BidAmountInterval
                : auction.StartPrice - auction.BidAmountInterval;

            if (maxAmount <= 0)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add($"New bids for this auctions are innacceptable.");

                return result;
            }

            if (newBid.Amount > maxAmount)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add($"Bid amount must be less or equal to ${maxAmount}");

                return result;
            }

            auction.Bids!.Add(newBid);
            auction.CurrentPrice = newBid.Amount;

            if (auction.AimPrice.HasValue && auction.AimPrice >= newBid.Amount)
            {
                auction.Status = AuctionStatus.Finished;
            }
            else if (auction.FinishMethod!.Name == AuctionFinishMethods.DynamicFinishMethod &&
                auction.FinishTimeIntervalInTicks.HasValue)
            {
                auction.FinishTime = DateTime.UtcNow.AddTicks(auction.FinishTimeIntervalInTicks.Value);
            }

            return result;
        }
    }
}
