namespace BidMasterOnline.Core.ServiceContracts
{
    public interface IUserAccessor
    {
        long UserId { get; }

        string UserName { get; }

        string Email { get; }

        string Role { get; }

        long? TryGetUserId();

        string? TryGetUserName();
    }
}
