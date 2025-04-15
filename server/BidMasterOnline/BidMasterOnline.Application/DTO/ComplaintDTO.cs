using BidMasterOnline.Application.Enums;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for complaint. (RESPONSE)
    /// </summary>
    public class ComplaintDTO
    {
        public Guid Id { get; set; }
        public Guid AccusingUserId { get; set; }
        public string AccusingUsername { get; set; }
        public Guid AccusedUserId { get; set; }
        public string AccusedUsername { get; set; }
        public Guid AuctionId { get; set; }
        public string AuctionName { get; set; }
        public Guid? CommentId { get; set; }
        public CommentDTO? Comment { get; set; }
        public ComplaintType ComplaintType { get; set; }
        public string ComplaintTypeDescription { get; set; }
        public string DateAndTime { get; set; }
        public string ComplaintText { get; set; }
    }
}
