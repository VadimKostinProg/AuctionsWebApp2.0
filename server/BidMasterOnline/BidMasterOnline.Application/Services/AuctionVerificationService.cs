using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Helpers;
using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Application.Services
{
    public class AuctionVerificationService : IAuctionVerificationService
    {
        private readonly IRepository _repository;
        private readonly INotificationsService _notificationsService;
        private readonly IImagesService _imagesService;

        public AuctionVerificationService(IRepository repository, INotificationsService notificationsService, 
            IImagesService imagesService)
        {
            _repository = repository;
            _notificationsService = notificationsService;
            _imagesService = imagesService;
        }

        public async Task ApproveAuctionAsync(Guid auctionId)
        {
            var auction = await _repository.FirstOrDefaultAsync<Auction>(x => x.Id == auctionId && !x.IsApproved);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            auction.IsApproved = true;
            auction.StartDateTime = DateTime.Now;
            auction.FinishDateTime = DateTime.Now.Add(new TimeSpan(auction.AuctionTime));

            await _repository.UpdateAsync(auction);

            _notificationsService.SendMessageOfApprovalAuctionToAuctionist(auction);
        }

        public async Task<AuctionDetailsDTO> GetNotApprovedAuctionDetailsByIdAsync(Guid id)
        {
            var auction = await _repository.FirstOrDefaultAsync<Auction>(x => x.Id == id && !x.IsApproved);

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

            return auctionDetailsDTO;
        }

        public async Task<ListModel<AuctionDTO>> GetNotApprovedAuctionsListAsync(SpecificationsDTO specifications)
        {
            if (specifications is null)
                throw new ArgumentNullException(nameof(specifications));

            var specification = new SpecificationBuilder<Auction>()
                .With(x => x.IsApproved == false)
                .OrderBy(x => x.StartDateTime, Enums.SortDirection.ASC)
                .WithPagination(specifications.PageSize, specifications.PageNumber)
                .Build();

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

        public async Task RejectAuctionAsync(RejectAuctionDTO request)
        {
            var auction = await _repository.GetByIdAsync<Auction>(request.AuctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.IsApproved)
                throw new InvalidOperationException("Cannot reject alrady approved auction.");

            await this.DeleteAuctionImagesAsync(auction);

            _notificationsService.SendMessageOfRejectionAuctionToAuctionist(auction, request.RejectionReason);

            await _repository.DeleteAsync(auction);
        }

        private async Task DeleteAuctionImagesAsync(Auction auction)
        {
            var imagePublicIds = auction.Images.Select(x => x.PublicId);

            foreach(var publicId in imagePublicIds)
            {
                await _imagesService.DeleteImageAsync(publicId);
            }
        }
    }
}
