using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for updating existant category. (REQUEST)
    /// </summary>
    public class UpdateCategoryDTO : CreateCategoryDTO
    {
        [Required]
        public Guid Id { get; set; }
    }
}
