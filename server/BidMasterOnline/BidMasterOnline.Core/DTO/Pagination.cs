namespace BidMasterOnline.Core.DTO
{
    public class Pagination
    {
        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public bool HasNext => CurrentPage < TotalPages;
    }
}
