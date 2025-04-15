using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Exceptions;
using BidMasterOnline.Application.Helpers;
using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace BidMasterOnline.Application.Services
{
    public class AuctionsService : IAuctionsService
    {
        private readonly IRepository _repository;
        private readonly IAuthService _authService;
        private readonly INotificationsService _notificationsService;
        private readonly IImagesService _imagesService;

        public AuctionsService(IRepository repository, IAuthService authService,
            INotificationsService notificationsService, IImagesService imagesService)
        {
            _repository = repository;
            _authService = authService;
            _notificationsService = notificationsService;
            _imagesService = imagesService;
        }

        public async Task CancelAuctionAsync(CancelAuctionDTO request)
        {
            if (request is null)
                throw new ArgumentException("Request object is null.");

            var auction = await _repository.GetByIdAsync<Auction>(request.AuctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.Status.Name == Enums.AuctionStatus.Canceled.ToString())
                throw new InvalidOperationException("Auction is already canceled.");

            if (auction.Status.Name == Enums.AuctionStatus.Finished.ToString())
                throw new InvalidOperationException("Auction is finished.");

            await this.PerformCancelingForAuctionAsync(auction);

            _notificationsService.SendMessageOfCancelingAuctionToAuctionist(auction, request.CancelationReason);
        }

        public async Task CancelOwnAuctionAsync(Guid auctionId)
        {
            var auction = await _repository.GetByIdAsync<Auction>(auctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.Status.Name == Enums.AuctionStatus.Canceled.ToString())
                throw new InvalidOperationException("Auction is already canceled.");

            if (auction.Status.Name == Enums.AuctionStatus.Finished.ToString())
                throw new InvalidOperationException("Auction is finished.");

            var user = await _authService.GetAuthenticatedUserEntityAsync();

            if (auction.AuctionistId != user.Id)
                throw new ForbiddenException("You cannot cancel the auction of other user.");

            await this.PerformCancelingForAuctionAsync(auction);
        }

        private async Task PerformCancelingForAuctionAsync(Auction auction)
        {
            var bids = auction.Bids;

            await _repository.DeleteManyAsync(bids);

            var status = await _repository.FirstOrDefaultAsync<AuctionStatus>(x =>
                x.Name == Enums.AuctionStatus.Canceled.ToString());

            auction.StatusId = status!.Id;

            await _repository.UpdateAsync(auction);

            await _repository.SaveChangesAsync();
        }

        public async Task<AuctionDTO> GetAuctionByIdAsync(Guid id)
        {
            var auction = await _repository.GetByIdAsync<Auction>(id);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            var auctionDTO = new AuctionDTO
            {
                Id = auction.Id,
                Name = auction.Name,
                Category = auction.Category.Name,
                AuctionistId = auction.AuctionistId,
                Auctionist = auction.Auctionist.Username,
                AuctionTime = ConvertHelper.TimeSpanTicksToString(auction.AuctionTime),
                FinishDateAndTime = auction.FinishDateTime.ToString("yyyy-MM-dd HH:mm"),
                StartPrice = auction.StartPrice,
                CurrentBid = auction.Bids.Any() ? auction.Bids.Max(x => x.Amount) : 0,
                ImageUrls = auction.Images.Select(x => x.Url).ToList()
            };

            return auctionDTO;
        }

        public async Task<AuctionDetailsDTO> GetAuctionDetailsByIdAsync(Guid id)
        {
            var auction = await _repository.FirstOrDefaultAsync<Auction>(x => x.Id == id && x.IsApproved);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            var auctionDetailsDTO = new AuctionDetailsDTO
            {
                Id = auction.Id,
                Name = auction.Name,
                Category = auction.Category.Name,
                AuctionistId = auction.AuctionistId,
                Auctionist = auction.Auctionist.Username,
                AuctionTime = ConvertHelper.TimeSpanTicksToString(auction.AuctionTime),
                FinishDateAndTime = auction.FinishDateTime.ToString("yyyy-MM-dd HH:mm"),
                StartPrice = auction.StartPrice,
                CurrentBid = auction.Bids.Any() ? auction.Bids.Max(x => x.Amount) : auction.StartPrice,
                ImageUrls = auction.Images.Select(x => x.Url).ToList(),
                StartDateAndTime = auction.StartDateTime.ToString("yyyy-MM-dd HH:mm"),
                LotDescription = auction.LotDescription,
                Score = auction.Scores.Any() ? Math.Round(auction.Scores.Average(x => x.Score), digits: 1) : 0,
                FinishTypeDescription = auction.FinishType.Description,
                Status = auction.Status.Name,
            };

            if (auction.Status.Name == Enums.AuctionStatus.Finished.ToString() && auction.Bids.Any())
            {
                var winningBid = auction.Bids.OrderByDescending(x => x.Amount).First();

                auctionDetailsDTO.WinnerId = winningBid.BidderId;
                auctionDetailsDTO.Winner = winningBid.Bidder.Username;
            }

            return auctionDetailsDTO;
        }

        public async Task<ListModel<AuctionDTO>> GetAuctionsListAsync(AuctionSpecificationsDTO specifications)
        {
            if (specifications is null)
                throw new ArgumentNullException("Specifications are null.");

            if (specifications.CategoryId is not null &&
                !await _repository.AnyAsync<Category>(x => x.Id == specifications.CategoryId && x.IsDeleted == false))
                throw new KeyNotFoundException("Category with such id does not exists.");

            if (specifications.AuctionistId is not null &&
                !await _repository.AnyAsync<User>(x => x.Id == specifications.AuctionistId))
                throw new KeyNotFoundException("User-auctionist with such id does not exist.");

            if ((specifications.Status is null || specifications.Status.Value != Enums.AuctionStatus.Finished) &&
                 specifications.WinnerId is not null)
                throw new ArgumentException("Cannot filter by winner while status is not finished.");

            if (specifications.WinnerId is not null &&
                !await _repository.AnyAsync<User>(x => x.Id == specifications.WinnerId))
                throw new KeyNotFoundException("User-winner with such id does not exist.");

            var specification = this.GetSpecification(specifications);

            var auctions = await _repository.GetAsync<Auction>(specification);

            var totalCount = await _repository.CountAsync<Auction>(specification.Predicate);

            var totalPages = (long)Math.Ceiling((double)totalCount / specifications.PageSize);

            var list = new ListModel<AuctionDTO>
            {
                List = auctions
                    .Select(auction => new AuctionDTO
                    {
                        Id = auction.Id,
                        Name = auction.Name,
                        Category = auction.Category.Name,
                        AuctionistId = auction.AuctionistId,
                        Auctionist = auction.Auctionist.Username,
                        AuctionTime = ConvertHelper.TimeSpanTicksToString(auction.AuctionTime),
                        FinishDateAndTime = auction.FinishDateTime.ToString("yyyy-MM-dd HH:mm"),
                        StartPrice = auction.StartPrice,
                        CurrentBid = auction.Bids.Any() ? auction.Bids.Max(x => x.Amount) : auction.StartPrice,
                        ImageUrls = auction.Images.Select(x => x.Url).ToList()
                    })
                    .ToList(),
                TotalPages = totalPages
            };

            return list;
        }

        private ISpecification<Auction> GetSpecification(AuctionSpecificationsDTO specifications)
        {
            var builder = new SpecificationBuilder<Auction>();

            builder.With(x => x.IsApproved == true);

            if (specifications.CategoryId is not null)
                builder.With(x => x.CategoryId == specifications.CategoryId);

            if (specifications.AuctionistId is not null)
                builder.With(x => x.AuctionistId == specifications.AuctionistId);

            if (specifications.MinStartPrice is not null)
                builder.With(x => x.StartPrice >= specifications.MinStartPrice && x.StartPrice <= specifications.MaxStartPrice!.Value);

            if (specifications.MinCurrentBid is not null)
                builder.With(x => x.Bids.Max(x => x.Amount) >= specifications.MinCurrentBid && x.Bids.Max(x => x.Amount) <= specifications.MaxCurrentBid!.Value);

            if (specifications.Status is not null)
                builder.With(x => x.Status.Name == specifications.Status.ToString());

            if (specifications.WinnerId is not null)
                builder.With(x => x.PaymentDeliveryOptions.WinnerId == specifications.WinnerId);

            if (!string.IsNullOrEmpty(specifications.SearchTerm))
                builder.With(x => x.Name.Contains(specifications.SearchTerm));

            if (!string.IsNullOrEmpty(specifications.SortField))
            {
                switch (specifications.SortField)
                {
                    case "popularity":
                        builder.OrderBy(x => x.Bids.Count(), specifications.SortDirection ?? Enums.SortDirection.DESC);
                        break;
                    case "dateAndTime":
                        builder.OrderBy(x => x.FinishDateTime, specifications.SortDirection ?? Enums.SortDirection.ASC);
                        break;
                }
            }

            builder.WithPagination(specifications.PageSize, specifications.PageNumber);

            return builder.Build();
        }

        public async Task PublishAuctionAsync(PublishAuctionDTO request)
        {
            if (request is null)
                throw new ArgumentNullException("Auction is null.");

            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentNullException("Auction name is blank.");

            if (string.IsNullOrEmpty(request.LotDescription))
                throw new ArgumentNullException("Lot description is blank.");

            if (request.FinishType == Enums.AuctionFinishType.IncreasingFinishTime && request.FinishTimeInterval is null)
                throw new ArgumentNullException("Finish time interval is blank.");

            var category = await _repository.GetByIdAsync<Category>(request.CategoryId);

            if (category is null)
                throw new KeyNotFoundException("Category with such id does not exist.");

            var auctionist = await _authService.GetAuthenticatedUserEntityAsync();

            if (auctionist.UserStatus.Name == Enums.UserStatus.Blocked.ToString())
                throw new ForbiddenException("Your account is blocked.");

            if (!auctionist.IsEmailConfirmed)
                throw new ForbiddenException("Your email is not confirmed.");

            var finishType = await _repository.FirstOrDefaultAsync<AuctionFinishType>(x =>
                x.Name == request.FinishType.ToString());

            var status = await _repository.FirstOrDefaultAsync<AuctionStatus>(x =>
                x.Name == Enums.AuctionStatus.Active.ToString());

            var auction = new Auction
            {
                Name = request.Name,
                AuctionistId = auctionist.Id,
                CategoryId = category.Id,
                LotDescription = request.LotDescription,
                StartDateTime = DateTime.Now,
                FinishDateTime = DateTime.Now.Add(request.AuctionTime),
                AuctionTime = request.AuctionTime.Ticks,
                FinishTypeId = finishType!.Id,
                FinishInterval = request.FinishType == Enums.AuctionFinishType.IncreasingFinishTime ? request.FinishTimeInterval!.Value.Ticks : null,
                StartPrice = request.StartPrice,
                StatusId = status!.Id,
                Auctionist = auctionist,
                Category = category,
                FinishType = finishType,
                Status = status,
            };

            await _repository.AddAsync(auction);

            await this.UploadAuctionImagesAsync(auction.Id, request.Images);

            _notificationsService.SendMessageOfPublishingAuctionToAuctionst(auction);
        }

        private async Task UploadAuctionImagesAsync(Guid auctionId, List<IFormFile> files)
        {
            foreach (var file in files)
            {
                var uploadResponse = await _imagesService.AddImageAsync(file, Enums.ImageType.ImageForAuction);

                var auctionImage = new AuctionImage
                {
                    AuctionId = auctionId,
                    Url = uploadResponse.SecureUrl.AbsoluteUri,
                    PublicId = uploadResponse.PublicId
                };

                await _repository.AddAsync(auctionImage);
            }
        }

        public async Task RecoverAuctionAsync(Guid auctionId)
        {
            var auction = await _repository.GetByIdAsync<Auction>(auctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.Status.Name == Enums.AuctionStatus.Active.ToString())
                throw new InvalidOperationException("Auction is active.");

            if (auction.Status.Name == Enums.AuctionStatus.Finished.ToString())
                throw new InvalidOperationException("Auction is finished.");

            var status = await _repository.FirstOrDefaultAsync<AuctionStatus>(x =>
                x.Name == Enums.AuctionStatus.Active.ToString());

            auction.StatusId = status!.Id;
            auction.StartDateTime = DateTime.Now;
            auction.FinishDateTime = DateTime.Now.Add(new TimeSpan(auction.AuctionTime));

            await _repository.UpdateAsync(auction);

            _notificationsService.SendMessageOfRecoveringAuctionToAuctionist(auction);
        }

        public async Task SetAuctionScoreAsync(SetAuctionScoreDTO request)
        {
            var user = await _authService.GetAuthenticatedUserEntityAsync();

            if (user.UserStatus.Name == Enums.UserStatus.Blocked.ToString())
                throw new ForbiddenException("Your account is blocked.");

            var auction = await _repository.FirstOrDefaultAsync<Auction>(x => x.Id == request.AuctionId && x.IsApproved);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.Status.Name == Enums.AuctionStatus.Canceled.ToString())
                throw new InvalidOperationException("Auction is canceled.");

            // If user has already set the score for auction before, update the score
            var existentScore = await _repository.FirstOrDefaultAsync<AuctionScore>(x =>
                x.UserId == user.Id && x.AuctionId == auction.Id);

            if (existentScore is not null)
            {
                existentScore.Score = request.Score;

                await _repository.UpdateAsync(existentScore);

                return;
            }

            // If user has not set the score for auction before, add new score
            var auctionScore = new AuctionScore
            {
                AuctionId = request.AuctionId,
                UserId = user.Id,
                Score = request.Score,
            };

            await _repository.AddAsync(auctionScore);
        }

        public async Task SetNextWinnerOfAuctionAsync(Guid auctionId)
        {
            var auction = await _repository.GetByIdAsync<Auction>(auctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.Status.Name == Enums.AuctionStatus.Canceled.ToString())
                throw new InvalidOperationException("Auction is canceled.");

            if (auction.Status.Name == Enums.AuctionStatus.Active.ToString())
                throw new InvalidOperationException("Cannot change the winner of auction before it is finished.");

            var bidsCount = auction.Bids.Count();

            if (bidsCount == 0)
            {
                throw new InvalidOperationException("Cannot set new winner of the auction as there are not any bids.");
            }

            await this.CancelWinnersBidForAuctionAsync(auction);

            if (bidsCount == 1)
            {
                await this.SetNoWinnersToAuctionAsync(auction);
            }
            else
            {
                await this.SetNextBidderAsWinnerAsync(auction);
            }
        }

        private async Task CancelWinnersBidForAuctionAsync(Auction auction)
        {
            var winnersBid = auction.Bids.First(x => x.IsWinning);

            _notificationsService.SendMessageOfCancelingTheBidToWinner(winnersBid);

            await _repository.DeleteAsync(winnersBid);
        }

        private async Task SetNextBidderAsWinnerAsync(Auction auction)
        {
            // Updating the winners bid
            var specification = new SpecificationBuilder<Bid>()
                .With(x => x.AuctionId == auction.Id)
                .OrderBy(x => x.Amount, Enums.SortDirection.DESC)
                .Build();

            var bids = await _repository.GetAsync<Bid>(specification);

            var newWinnersBid = bids.First();

            newWinnersBid.IsWinning = true;

            await _repository.UpdateAsync(newWinnersBid);

            // Updating sell and delivery options
            var paymentDeliveryOptions = auction.PaymentDeliveryOptions;

            paymentDeliveryOptions!.WinnerId = newWinnersBid.BidderId;
            paymentDeliveryOptions.IBAN =
            paymentDeliveryOptions.Country =
            paymentDeliveryOptions.City =
            paymentDeliveryOptions.ZipCode =
            paymentDeliveryOptions.Waybill = null;
            paymentDeliveryOptions.AreDeliveryOptionsSet =
            paymentDeliveryOptions.ArePaymentOptionsSet =
            paymentDeliveryOptions.IsDeliveryConfirmed =
            paymentDeliveryOptions.IsPaymentConfirmed = false;

            await _repository.UpdateAsync(paymentDeliveryOptions);

            // Sending a letter to new winner
            var user = await _repository.GetByIdAsync<User>(newWinnersBid.BidderId);

            _notificationsService.SendMessageOfDeliveryOptionsSetToWinner(auction, user);
        }

        private async Task SetNoWinnersToAuctionAsync(Auction auction)
        {
            var paymentDeliveryOptions = auction.PaymentDeliveryOptions;

            paymentDeliveryOptions!.WinnerId = null;
            paymentDeliveryOptions.IBAN =
            paymentDeliveryOptions.Country =
            paymentDeliveryOptions.City =
            paymentDeliveryOptions.ZipCode =
            paymentDeliveryOptions.Waybill = null;
            paymentDeliveryOptions.AreDeliveryOptionsSet =
            paymentDeliveryOptions.ArePaymentOptionsSet =
            paymentDeliveryOptions.IsDeliveryConfirmed =
            paymentDeliveryOptions.IsPaymentConfirmed = false;

            await _repository.UpdateAsync(paymentDeliveryOptions);

            _notificationsService.SendMessageOfNoWinnersOfAuctionToAuctionist(auction);
        }

        public async Task<IEnumerable<AuctionDTO>> GetFinishedAuctionsWithNotConfirmedOptionsAsync(Enums.AuctionParticipant participant)
        {
            var user = await _authService.GetAuthenticatedUserAsync();

            var builder = new SpecificationBuilder<Auction>()
                .With(x => x.Status.Name == Enums.AuctionStatus.Finished.ToString());

            switch (participant)
            {
                case Enums.AuctionParticipant.Auctionist:
                    builder.With(x => x.AuctionistId == user.Id);
                    break;
                case Enums.AuctionParticipant.Auctioner:
                    builder.With(x => x.PaymentDeliveryOptions.WinnerId == user.Id);
                    break;
            }

            var specification = builder
                .With(x => !x.PaymentDeliveryOptions.IsDeliveryConfirmed || !x.PaymentDeliveryOptions.IsDeliveryConfirmed)
                .OrderBy(x => x.FinishDateTime, Enums.SortDirection.ASC)
                .Build();

            var auctions = await _repository.GetAsync<Auction>(specification);

            var list = auctions
                .Select(auction => new AuctionDTO
                {
                    Id = auction.Id,
                    Name = auction.Name,
                    Category = auction.Category.Name,
                    AuctionistId = auction.AuctionistId,
                    Auctionist = auction.Auctionist.Username,
                    AuctionTime = ConvertHelper.TimeSpanTicksToString(auction.AuctionTime),
                    FinishDateAndTime = auction.FinishDateTime.ToString("yyyy-mm-dd HH:mm"),
                    StartPrice = auction.StartPrice,
                    CurrentBid = auction.Bids.Any() ? auction.Bids.Max(x => x.Amount) : auction.StartPrice,
                    ImageUrls = auction.Images.Select(x => x.Url).ToList()
                })
                .ToList();

            return list;
        }
    }
}