namespace Users.Service.API.DTO.Moderator
{
    public class ModeratorSummaryDTO
    {
        public long Id { get; set; }

        public required string Username { get; set; }

        public required string FullName { get; set; }

        public required string Email { get; set; }
    }
}
