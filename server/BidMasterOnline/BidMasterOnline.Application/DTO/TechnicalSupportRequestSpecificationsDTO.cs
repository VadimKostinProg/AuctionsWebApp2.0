namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for technical support requests specifications. (REQUEST)
    /// </summary>
    public class TechnicalSupportRequestSpecificationsDTO : SpecificationsDTO
    {
        public bool IsHandled { get; set; } = false;
    }
}
