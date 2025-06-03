namespace Auctions.Service.API.DTO.Moderator
{
    public class SpecificationsDTO
    {
        public string? Search { get; set; }

        public int PageSize { get; set; } = 15;

        public int PageNumber { get; set; } = 1;

        public bool IncludeDeleted { get; set; } = false;
    }
}
