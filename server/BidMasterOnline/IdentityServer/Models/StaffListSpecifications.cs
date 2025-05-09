namespace IdentityServer.Models
{
    public class StaffListSpecifications
    {
        public string Search { get; set; }
        public string SortColumn { get; set; } = "Username";
        public string SortDirection { get; set; } = "asc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}
