namespace Users.Service.API.DTO.Moderator
{
    public class BlockUserDTO
    {
        public long UserId { get; set; }

        public required string BlockingReason { get; set; }

        public int? BlockingPeriodInDays { get; set; }
    }
}
