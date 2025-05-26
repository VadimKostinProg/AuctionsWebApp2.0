using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;
using Bids.Service.API.ServiceContracts.Participant;

namespace Bids.Service.API.Services.Participant
{
    public class EnglishBidsPlacingStrategy : IBidsPlacingStrategy
    {
        public ServiceResult PlaceNewBid(Bid newBid, Auction auction)
        {
            ServiceResult result = new();

            if (auction.Status != AuctionStatus.Active || auction.FinishTime <= DateTime.UtcNow)
            {
                result.IsSuccessfull = false;
                result.Errors.Add($"Could not place bid on inactive auction.");

                return result;
            }

            if (auction.Bids!.Any() && auction.Bids!.OrderByDescending(x => x.CreatedAt).First().BidderId == newBid.BidderId)
            {
                result.IsSuccessfull = false;
                result.Errors.Add($"Could not place two bids one after another.");

                return result;
            }

            decimal minAmount = auction.Bids!.Any()
                ? auction.Bids!.Where(e => !e.Deleted).Max(e => e.Amount) + auction.BidAmountInterval
                : auction.StartPrice + auction.BidAmountInterval;

            if (newBid.Amount < minAmount)
            {
                result.IsSuccessfull = false;
                result.Errors.Add($"Bid amount must be more or equal to ${minAmount}");

                return result;
            }

            auction.Bids!.Add(newBid);
            auction.CurrentPrice = newBid.Amount;

            if (auction.AimPrice.HasValue && newBid.Amount >= auction.AimPrice)
            {
                auction.FinishTime = DateTime.UtcNow;
                auction.Status = AuctionStatus.Finished;

                auction.WinnerId = newBid.BidderId;

                // TODO: send messages
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
