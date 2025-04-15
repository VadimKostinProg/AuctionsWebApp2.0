using BidMasterOnline.Application.DTO;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BidMasterOnline.Tests.UserManagerTests
{
    /// <summary>
    /// Class for IUserManager.GetUserByIdAsync method unit tests.
    /// </summary>
    public class GetUserByIdAsyncTests : UserManagerTestsBase
    {
        [Fact]
        public async Task GetUserByIdAsync_IncorrectId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.GetByIdAsync<User>(It.IsAny<Guid>(), false))
                .ReturnsAsync(null as User);

            // Assert
            var action = async () =>
            {
                // Act
                var user = await service.GetUserByIdAsync(idToPass);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task GetUserByIdAsync_ValidId_ReturnsValidUser()
        {
            // Arrange
            var user = GetTestUser();

            var idToPass = user.Id;

            var expectedUser = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = Enum.Parse<Application.Enums.UserRole>(user.Role.Name),
                ImageUrl = user.ImageUrl,
                Status = Enum.Parse<Application.Enums.UserStatus>(user.UserStatus.Name)
            };

            repositoryMock.Setup(x => x.GetByIdAsync<User>(It.IsAny<Guid>(), false))
                .ReturnsAsync(user);

            // Act
            var actualUser = await service.GetUserByIdAsync(idToPass);

            // Assert
            actualUser.Should().NotBeNull();
            actualUser.Should().BeEquivalentTo(expectedUser);
        }
    }
}
