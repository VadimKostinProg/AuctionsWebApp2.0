namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for comments. (RESPONSE)
    /// </summary>
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public Guid AuctionId { get; set; }
        public string AuctionName { get; set; } 
        public string DateAndTime { get; set; }
        public string CommentText { get; set; }        
        public bool IsDeleted { get; set; }
    }
}
