using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for setting new technical support request. (REQUEST)
    /// </summary>
    public class SetTechnicalSupportRequestDTO
    {
        [Required]
        [MaxLength(500)]
        public string RequestText { get; set; }
    }
}
