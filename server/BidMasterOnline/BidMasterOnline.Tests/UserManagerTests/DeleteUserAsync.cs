using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.UserManagerTests
{
    /// <summary>
    /// Class for IUserManager.DeleteUserAsync method unit tests.
    /// </summary>
    public class DeleteUserAsync : UserManagerTestsBase
    {
        [Fact]
        public async Task DeleteUserAsync_IncorrectId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.GetByIdAsync<User>(idToPass, false))
                .ReturnsAsync(null as User);

            // Assert
            var action = async () =>
            {
                // Act
                await service.DeleteUserAsync(idToPass);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteUserAsync_UserIsDeleted_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Deleted);

            var idToPass = user.Id;

            repositoryMock.Setup(x => x.GetByIdAsync<User>(idToPass, false))
                .ReturnsAsync(user);

            // Assert
            var action = async () =>
            {
                // Act
                await service.DeleteUserAsync(idToPass);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task DeleteUserAsync_ValidId_SuccessfullDeleting()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Active);

            var idToPass = user.Id;

            var status = new UserStatus
            {
                Name = "Deleted"
            };

            repositoryMock.Setup(x => x.GetByIdAsync<User>(idToPass, false))
                .ReturnsAsync(user);
            repositoryMock.Setup(x => x.FirstOrDefaultAsync<UserStatus>(It.IsAny<Expression<Func<UserStatus, bool>>>(), false))
                .ReturnsAsync(status);
            repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Assert
            var action = async () =>
            {
                // Act
                await service.DeleteUserAsync(idToPass);
            };

            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteUserAsync_NoIdPassed_SuccessfullDeletingAuthenticatedUser()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Active);

            var status = new UserStatus
            {
                Name = "Deleted"
            };

            authServiceMock.Setup(x => x.GetAuthenticatedUserEntityAsync())
                .ReturnsAsync(user);
            repositoryMock.Setup(x => x.FirstOrDefaultAsync<UserStatus>(It.IsAny<Expression<Func<UserStatus, bool>>>(), false))
                .ReturnsAsync(status);
            repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Assert
            var action = async () =>
            {
                // Act
                await service.DeleteUserAsync();
            };

            await action.Should().NotThrowAsync();
        }
    }
}
