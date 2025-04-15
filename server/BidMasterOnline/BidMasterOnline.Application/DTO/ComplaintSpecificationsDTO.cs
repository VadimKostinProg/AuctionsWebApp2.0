using BidMasterOnline.Application.Enums;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for complaint specifications for filtering, sorting and pagination. (REQUEST)
    /// </summary>
    public class ComplaintSpecificationsDTO : SpecificationsDTO
    {
        public ComplaintType Type { get; set; }
    }
}
