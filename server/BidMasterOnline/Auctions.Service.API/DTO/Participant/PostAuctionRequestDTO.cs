using System.ComponentModel.DataAnnotations;

namespace Auctions.Service.API.DTO.Participant
{
    public class PostAuctionRequestDTO
    {
        public long AuctionCategoryId { get; set; }

        public long AuctionTypeId { get; set; }

        public long AuctionFinishMethodId { get; set; }

        public required string LotTitle { get; set; }

        public required string LotDescription { get; set; }

        public TimeSpan RequestedAuctionTime { get; set; }

        [Range(100, 10e9)]
        public decimal StartPrice { get; set; }

        public DateTime? RequestedStartTime { get; set; }

        public TimeSpan? FinishTimeInterval { get; set; }

        [Range(10, 10e9)]
        public decimal BidAmountInterval { get; set; }

        [MinLength(1)]
        public List<IFormFile> Images { get; set; } = [];
    }
}
