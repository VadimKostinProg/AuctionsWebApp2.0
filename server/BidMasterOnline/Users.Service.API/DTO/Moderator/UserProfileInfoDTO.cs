namespace Users.Service.API.DTO.Moderator
{
    public class UserProfileInfoDTO : UserProfileSummaryInfoDTO
    {
        public required string Role { get; set; }

        public DateTime DateOfBirth { get; set; }

        public double? AverageScore { get; set; }

        public int TotalAuctions { get; set; }

        public int CompletedAuctions { get; set; }

        public int TotalWins { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool IsPaymentMethodAttached { get; set; }

        public string? BlockingReason { get; set; }

        public DateTime? UnblockDateTime { get; set; }
    }
}
