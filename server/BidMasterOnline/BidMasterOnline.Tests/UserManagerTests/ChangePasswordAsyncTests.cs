using AutoFixture;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Exceptions;
using BidMasterOnline.Application.Helpers;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BidMasterOnline.Tests.UserManagerTests
{
    /// <summary>
    /// Class for IUserManager.ChangePasswordAsync method unit tests.
    /// </summary>
    public class ChangePasswordAsyncTests : UserManagerTestsBase
    {
        [Fact]
        public async Task ChangePasswordAsync_NullPassed_ThrowsArgumentNullException()
        {
            // Arrange
            ChangePasswordDTO changePasswordDTO = null;

            // Assert
            var action = async () =>
            {
                // Act
                await service.ChangePasswordAsync(changePasswordDTO);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ChangePasswordAsync_CurrentPasswordIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var authenticatedUser = GetTestUser();

            ChangePasswordDTO changePasswordDTO = fixture.Build<ChangePasswordDTO>()
                .With(x => x.CurrentPassword, string.Empty)
                .Create();

            authServiceMock.Setup(x => x.GetAuthenticatedUserEntityAsync())
                .ReturnsAsync(authenticatedUser);

            // Assert
            var action = async () =>
            {
                // Act
                await service.ChangePasswordAsync(changePasswordDTO);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ChangePasswordAsync_NewPasswordIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var authenticatedUser = GetTestUser();

            ChangePasswordDTO changePasswordDTO = fixture.Build<ChangePasswordDTO>()
                .With(x => x.NewPassword, string.Empty)
                .Create();

            authServiceMock.Setup(x => x.GetAuthenticatedUserEntityAsync())
                .ReturnsAsync(authenticatedUser);

            // Assert
            var action = async () =>
            {
                // Act
                await service.ChangePasswordAsync(changePasswordDTO);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ChangePasswordAsync_UserIsDeleted_ThrowsForbiddenException()
        {
            // Arrange
            var authenticatedUser = GetTestUser(Application.Enums.UserStatus.Deleted);

            ChangePasswordDTO changePasswordDTO = fixture.Create<ChangePasswordDTO>();

            authServiceMock.Setup(x => x.GetAuthenticatedUserEntityAsync())
                .ReturnsAsync(authenticatedUser);

            // Assert
            var action = async () =>
            {
                // Act
                await service.ChangePasswordAsync(changePasswordDTO);
            };

            await action.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task ChangePasswordAsync_CurrentPasswordIncorrect_ThrowsArgumentException()
        {
            // Arrange
            var authenticatedUser = GetTestUser();

            ChangePasswordDTO changePasswordDTO = fixture.Create<ChangePasswordDTO>();

            authServiceMock.Setup(x => x.GetAuthenticatedUserEntityAsync())
                .ReturnsAsync(authenticatedUser);

            // Assert
            var action = async () =>
            {
                // Act
                await service.ChangePasswordAsync(changePasswordDTO);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task ChangePasswordAsync_CorrectModel_SuccessfullPasswordChanging()
        {
            // Arrange
            var password = "test-password"; 
            var authenticatedUser = GetTestUser();

            authenticatedUser.PasswordSalt = CryptographyHelper.GenerateSalt(size: 128);
            authenticatedUser.PasswordHashed = CryptographyHelper.Hash(password, authenticatedUser.PasswordSalt);

            ChangePasswordDTO changePasswordDTO = fixture.Build<ChangePasswordDTO>()
                .With(x => x.CurrentPassword, password)
                .Create();

            authServiceMock.Setup(x => x.GetAuthenticatedUserEntityAsync())
                .ReturnsAsync(authenticatedUser);
            repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Assert
            var action = async () =>
            {
                // Act
                await service.ChangePasswordAsync(changePasswordDTO);
            };

            await action.Should().NotThrowAsync();
        }
    }
}
