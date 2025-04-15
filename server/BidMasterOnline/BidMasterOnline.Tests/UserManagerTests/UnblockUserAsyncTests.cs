using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.UserManagerTests
{
    /// <summary>
    /// Class for IUserManager.UnblockUserAsync method unit tests.
    /// </summary>
    public class UnblockUserAsyncTests : UserManagerTestsBase
    {
        [Fact]
        public async Task UnblockUserAsync_IncorrectId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.GetByIdAsync<User>(It.IsAny<Guid>(), false))
                .ReturnsAsync(null as User);

            // Assert
            var action = async () =>
            {
                // Act
                await service.UnblockUserAsync(idToPass);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UnblockUserAsync_UserIsActive_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Active);

            var idToPass = user.Id;

            repositoryMock.Setup(x => x.GetByIdAsync<User>(It.IsAny<Guid>(), false))
                .ReturnsAsync(user);

            // Assert
            var action = async () =>
            {
                // Act
                await service.UnblockUserAsync(idToPass);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UnblockUserAsync_UserIsDeleted_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Deleted);

            var idToPass = user.Id;

            repositoryMock.Setup(x => x.GetByIdAsync<User>(It.IsAny<Guid>(), false))
                .ReturnsAsync(user);

            // Assert
            var action = async () =>
            {
                // Act
                await service.UnblockUserAsync(idToPass);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UnblockUserAsync_UserIsBlocked_SuccessfullUnblocking()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Blocked);

            var idToPass = user.Id;

            var status = new UserStatus()
            {
                Name = Application.Enums.UserStatus.Blocked.ToString()
            };

            repositoryMock.Setup(x => x.GetByIdAsync<User>(It.IsAny<Guid>(), false))
                .ReturnsAsync(user);
            repositoryMock.Setup(x => x.UpdateAsync<User>(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            repositoryMock.Setup(x => x.FirstOrDefaultAsync<UserStatus>(It.IsAny<Expression<Func<UserStatus, bool>>>(), false))
                .ReturnsAsync(status);

            // Assert
            var action = async () =>
            {
                // Act
                await service.UnblockUserAsync(idToPass);
            };

            await action.Should().NotThrowAsync();
        }
    }
}
