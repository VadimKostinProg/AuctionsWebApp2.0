using System.ComponentModel.DataAnnotations;

namespace Users.Service.API.DTO.Participant
{
    public class ResetPasswordDTO
    {
        public required string CurrentPassword { get; set; }

        public required string NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        public required string RepeatNewPassword { get; set; }
    }
}
