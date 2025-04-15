using BidMasterOnline.Application.Enums;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// Specifications for searching, sorting and paginating items. (REQUEST)
    /// </summary>
    public class SpecificationsDTO
    {
        // Searching
        public string? SearchTerm { get; set; }

        // Sorting
        public string? SortField { get; set; }
        public SortDirection? SortDirection { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}
