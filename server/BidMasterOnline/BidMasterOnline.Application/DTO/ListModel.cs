namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for list of paged items. (RESPONSE)
    /// </summary>
    /// <typeparam name="T">Type of item.</typeparam>
    public class ListModel<T>
    {
        public List<T> List { get; set; }
        public long TotalPages { get; set; }
    }
}
