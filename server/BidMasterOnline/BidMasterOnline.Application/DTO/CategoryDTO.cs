namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for category. (RESPONSE)
    /// </summary>
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
