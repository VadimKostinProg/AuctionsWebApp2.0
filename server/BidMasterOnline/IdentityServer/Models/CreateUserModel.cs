namespace IdentityServer.Models
{
    public class CreateUserModel
    {
        public required string Username { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; } 
        public DateTime DateOfBirth { get; set; }
        public required string Password { get; set; }
    }
}
