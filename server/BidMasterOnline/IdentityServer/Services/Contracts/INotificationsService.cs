namespace IdentityServer.Services.Contracts
{
    public interface INotificationsService
    {
        Task SendEmailConfirmationMessageAsync(string email, string fullName, string code);
    }
}
