namespace BidMasterOnline.Domain.Models
{
    public class ListModel<T>
    {
        public List<T> Items { get; set; } = [];

        public required Pagination Pagination { get; set; }
    }
}
