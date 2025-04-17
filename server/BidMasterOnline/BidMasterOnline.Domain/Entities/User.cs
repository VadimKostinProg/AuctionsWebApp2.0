using BidMasterOnline.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BidMasterOnline.Domain.Entities
{
    public class User : EntityBase
    {
        public long RoleId { get; set; }

        [MaxLength(30)]
        public required string Username { get; set; }
        
        [MaxLength(50)]
        public required string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(30)]
        public required string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public required string PasswordHashed { get; set; }

        public required string PasswordSalt { get; set; }

        public DateTime? UnblockDateTime { get; set; }

        public double AverageScore { get; set; }

        public UserStatus Status { get; set; }

        public string? ImageUrl { get; set; }

        public string? ImagePublicId { get; set; }

        public Role? Role { get; set; }
    }
}
