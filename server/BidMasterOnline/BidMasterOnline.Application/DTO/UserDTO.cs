using BidMasterOnline.Application.Enums;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for users. (RESPONSE)
    /// </summary>
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Role { get; set; }
        public string? ImageUrl { get; set; }
        public string Status { get; set; }
    }
}
