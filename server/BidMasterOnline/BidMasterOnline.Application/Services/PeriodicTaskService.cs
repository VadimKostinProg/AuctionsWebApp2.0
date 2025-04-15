using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BidMasterOnline.Application.Services
{
    public class PeriodicTaskService : IPeriodicTaskService
    {
        private readonly IRepository _repository;
        private readonly INotificationsService _notificationsService;
        private readonly ILogger<PeriodicTaskService> _logger;

        public PeriodicTaskService(IRepository repository, INotificationsService notificationsService, 
            ILogger<PeriodicTaskService> logger)
        {
            _repository = repository;
            _notificationsService = notificationsService;
            _logger = logger;
        }

        public async Task PerformPeriodicTaskAsync()
        {
            _logger.LogInformation("--> PeriodicTaskService: Performing periodic operation has started.");

            await ApplyAuctionFinishingAsync();
            await ApplyUnblockingUsers();

            _logger.LogInformation("--> PeriodicTaskService: Performing periodic operation has finished.");
        }

        /// <summary>
        /// Applyes finished status to all actually finished auctions and sends the notification to the aucitonist and winner.
        /// </summary>
        private async Task ApplyAuctionFinishingAsync()
        {
            var now = DateTime.Now;

            var specification = new SpecificationBuilder<Auction>()
                .With(x => x.IsApproved)
                .With(x => x.Status.Name == Enums.AuctionStatus.Active.ToString())
                .With(x => x.FinishDateTime < now)
                .Build();

            var auctionsToFinish = await _repository.GetAsync(specification, disableTracking: false);

            var finishedStatus = await _repository.FirstOrDefaultAsync<AuctionStatus>(x => x.Name == Enums.AuctionStatus.Finished.ToString(), disableTracking: false);

            foreach (var auction in auctionsToFinish)
            {
                auction.StatusId = finishedStatus.Id;
                await _repository.UpdateAsync(auction);

                var paymentDeliveryOptions = new AuctionPaymentDeliveryOptions
                {
                    AuctionId = auction.Id,
                };

                if (auction.Bids.Any())
                {
                    var winningBid = auction.Bids.OrderByDescending(x => x.DateAndTime).First();

                    winningBid.IsWinning = true;

                    await _repository.UpdateAsync(winningBid);

                    var winner = winningBid.Bidder;

                    paymentDeliveryOptions.WinnerId = winner.Id;

                    _notificationsService.SendMessageOfPaymentOptionsSetToAuctionist(auction);
                    _notificationsService.SendMessageOfDeliveryOptionsSetToWinner(auction, winner);
                }
                else
                {
                    _notificationsService.SendMessageOfNoWinnersOfAuctionToAuctionist(auction);
                }

                await _repository.AddAsync(paymentDeliveryOptions);
            }

            _logger.LogInformation($"--> PeriodicTaskService: {auctionsToFinish.Count()} auctions has been applyed finished status to.");
        }

        /// <summary>
        /// Applyes unblocking of the temporary blocked users.
        /// </summary>
        private async Task ApplyUnblockingUsers()
        {
            var now = DateTime.Now;

            var specification = new SpecificationBuilder<User>()
                .With(x => x.UnblockDateTime != null && x.UnblockDateTime < now)
                .Build();

            var blockedUsers = await _repository.GetAsync<User>(specification);

            var activeStatus = await _repository.FirstOrDefaultAsync<UserStatus>(x => 
                x.Name == Enums.UserStatus.Active.ToString());

            foreach(var blockedUser in blockedUsers)
            {
                blockedUser.UserStatusId = activeStatus!.Id;
                blockedUser.UnblockDateTime = null;

                await _repository.UpdateAsync(blockedUser);

                _notificationsService.SendMessageOfUnblockingAccountToUser(blockedUser);
            }

            _logger.LogInformation($"--> PeriodicTaskService: {blockedUsers.Count()} users has been unblocked.");
        }
    }
}
