namespace Users.Service.API.DTO.Moderator
{
    public class BlockUserDTO
    {
        public required string BlockingReason { get; set; }

        public int? BlockingPeriodInDays { get; set; }
    }
}
