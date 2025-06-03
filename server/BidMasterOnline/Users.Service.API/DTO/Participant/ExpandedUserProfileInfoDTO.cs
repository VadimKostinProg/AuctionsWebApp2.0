namespace Users.Service.API.DTO.Participant
{
    public class ExpandedUserProfileInfoDTO : UserProfileInfoDTO
    {
        public bool IsEmailConfirmed { get; set; }

        public bool IsPaymentMethodAttached { get; set; }

        public DateTime? UnblockDateTime { get; set; }
    }
}
