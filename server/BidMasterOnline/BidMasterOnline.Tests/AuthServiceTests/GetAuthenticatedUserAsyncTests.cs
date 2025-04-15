using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Linq.Expressions;
using System.Security.Authentication;

namespace BidMasterOnline.Tests.AuthServiceTests
{
    /// <summary>
    /// Class for IAuthService.GetAuthenticatedUserAsync method unit tests.
    /// </summary>
    public class GetAuthenticatedUserAsyncTests : AuthServiceTestsBase
    {
        [Fact]
        public async Task GetAuthenticatedUserAsync_SessionContainsNoUserData_ThrowsAuthenticationException()
        {
            // Arrange
            sessionContext.UserId = null;

            // Assert
            var action = async () =>
            {
                // Act
                var user = await service.GetAuthenticatedUserAsync();
            };

            await action.Should().ThrowAsync<AuthenticationException>();
        }

        [Fact]
        public async Task GetAuthenticatedUserAsync_SessionContainsIncorrectClaims_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            sessionContext.UserId = Guid.NewGuid();

            repositoryMock.Setup(x => x.FirstOrDefaultAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), false))
                .ReturnsAsync(null as User);

            // Assert
            var action = async () =>
            {
                // Act
                var user = await service.GetAuthenticatedUserAsync();
            };

            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task GetAuthenticatedUserAsync_ValidSessionClaims_ReturnsAuthenticatedUser()
        {
            // Arrange
            var user = this.GetTestUser(Application.Enums.UserStatus.Active);

            sessionContext.UserId = user.Id;

            repositoryMock.Setup(x => x.FirstOrDefaultAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), false))
                .ReturnsAsync(user);

            var authenticatedUser = await service.GetAuthenticatedUserAsync();

            // Assert
            authenticatedUser.Should().NotBeNull();
            authenticatedUser.Id.Should().Be(user.Id);
            authenticatedUser.Username.Should().Be(user.Username);
            authenticatedUser.FullName.Should().Be(user.FullName);
            authenticatedUser.Email.Should().Be(user.Email);
            authenticatedUser.ImageUrl.Should().Be(user.ImageUrl);
            authenticatedUser.Status.ToString().Should().Be(user.UserStatus.Name);
        }
    }
}
