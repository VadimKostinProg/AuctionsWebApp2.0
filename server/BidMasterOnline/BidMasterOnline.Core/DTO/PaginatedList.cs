namespace BidMasterOnline.Core.DTO
{
    public class PaginatedList<T>
    {
        public required List<T> Items { get; set; }

        public required Pagination Pagination { get; set; }
    }
}
