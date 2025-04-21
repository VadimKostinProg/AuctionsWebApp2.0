namespace BidMasterOnline.Core.ServiceContracts
{
    public interface IUserAccessor
    {
        public long UserId { get; }

        public string UserName { get; }

        public string Email { get; }

        public string Role { get; }
    }
}
