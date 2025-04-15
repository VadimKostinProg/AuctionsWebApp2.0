using BidMasterOnline.Application.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for publishing auction for verification. (REQUEST)
    /// </summary>
    public class PublishAuctionDTO
    {
        [Required(ErrorMessage = "Lot name is required.")]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Category is required.")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Lot description is required.")]
        [MaxLength(300)]
        public string LotDescription { get; set; } = null!;

        [Required(ErrorMessage = "Auction finish type is required.")]
        public AuctionFinishType FinishType { get; set; }

        [Required(ErrorMessage = "Auction time is required.")]
        public TimeSpan AuctionTime { get; set; }

        [AllowNull]
        public TimeSpan? FinishTimeInterval { get; set; }

        [Required(ErrorMessage = "Start price is required.")]
        [Range(100, 10e9)]
        public decimal StartPrice { get; set; }

        [Required]
        [MinLength(1)]
        public List<IFormFile> Images { get; set; }
    }
}
