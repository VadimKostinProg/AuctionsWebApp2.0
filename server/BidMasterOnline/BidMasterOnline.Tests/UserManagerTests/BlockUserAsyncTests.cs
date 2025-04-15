using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.UserManagerTests
{
    /// <summary>
    /// Class for IUserManager.BlockUserAsync method unit tests.
    /// </summary>
    public class BlockUserAsyncTests : UserManagerTestsBase
    {
        [Fact]
        public async Task BlockUserAsync_IncorrectId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.GetByIdAsync<User>(It.IsAny<Guid>(), false))
                .ReturnsAsync(null as User);

            // Assert
            var action = async () =>
            {
                // Act
                await service.BlockUserAsync(idToPass, null);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task BlockUserAsync_UserIsBlocked_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Blocked);

            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.GetByIdAsync<User>(It.IsAny<Guid>(), false))
                .ReturnsAsync(user);

            // Assert
            var action = async () =>
            {
                // Act
                await service.BlockUserAsync(idToPass, null);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task BlockUserAsync_UserIsDeleted_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Deleted);

            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.GetByIdAsync<User>(It.IsAny<Guid>(), false))
                .ReturnsAsync(user);

            // Assert
            var action = async () =>
            {
                // Act
                await service.BlockUserAsync(idToPass, null);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task BlockUserAsync_UserActive_SuccessfullBlockingUser()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Active);

            var idToPass = Guid.NewGuid();

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
                await service.BlockUserAsync(idToPass, null);
            };

            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task BlockUserAsync_IncorrectDaysNumber_ThrowsArgumentExcpetion()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Active);

            var idToPass = Guid.NewGuid();

            var status = new UserStatus()
            {
                Name = Application.Enums.UserStatus.Blocked.ToString()
            };

            repositoryMock.Setup(x => x.GetByIdAsync<User>(It.IsAny<Guid>(), false))
                .ReturnsAsync(user);
            repositoryMock.Setup(x => x.FirstOrDefaultAsync<UserStatus>(It.IsAny<Expression<Func<UserStatus, bool>>>(), false))
                .ReturnsAsync(status);

            int days = -1;

            // Assert
            var action = async () =>
            {
                // Act
                await service.BlockUserAsync(idToPass, days);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task BlockUserAsync_ValidDaysNumber_SuccessfullBlockingUser()
        {
            // Arrange
            var user = GetTestUser(Application.Enums.UserStatus.Active);

            var idToPass = Guid.NewGuid();

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

            int days = 1;

            // Assert
            var action = async () =>
            {
                // Act
                await service.BlockUserAsync(idToPass, days);
            };

            await action.Should().NotThrowAsync();
        }
    }
}
