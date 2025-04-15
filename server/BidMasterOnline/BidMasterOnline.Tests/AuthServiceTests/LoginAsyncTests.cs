using AutoFixture;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Helpers;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.AuthServiceTests
{
    /// <summary>
    /// Class for IAuthService.LoginAsync method unit tests.
    /// </summary>
    public class LoginAsyncTests : AuthServiceTestsBase
    {
        [Fact]
        public async Task LoginAsync_NullPassed_ThrowsArgumentNullException()
        {
            // Arrange
            LoginDTO? login = null;

            // Assert
            var action = async () =>
            {
                var response = await service.LoginAsync(login);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task LoginAsync_NullPassed_ThrowsKeyNotFoundException()
        {
            // Arrange
            var login = fixture.Create<LoginDTO>();

            repositoryMock.Setup(x => x.FirstOrDefaultAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), false))
                .ReturnsAsync(null as User);

            // Assert
            var action = async () =>
            {
                var response = await service.LoginAsync(login);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task LoginAsync_PasswordIncorrect_ThrowArgumentException()
        {
            // Arrange
            var user = this.GetTestUser();

            var login = fixture.Build<LoginDTO>()
                .With(x => x.UserName, user.Username)
                .Create();

            repositoryMock.Setup(x => x.FirstOrDefaultAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), false))
                .ReturnsAsync(user);

            // Assert
            var action = async () =>
            {
                var response = await service.LoginAsync(login);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task LoginAsync_CorrectLogin_ReturnsAuthenticationResponse()
        {
            // Arrange
            configurationMock.Setup(x => x["Jwt:EXPIRATION_MINUTES"])
                .Returns("10");
            configurationMock.Setup(x => x["Jwt:Key"])
                .Returns("my-secret-key-for-unit-tests-12324653");
            configurationMock.Setup(x => x["Jwt:Issuer"])
                .Returns("issuer");
            configurationMock.Setup(x => x["Jwt:Audience"])
                .Returns("audience");

            var user = this.GetTestUser();

            var inputPassword = fixture.Create<string>();

            user.PasswordSalt = CryptographyHelper.GenerateSalt(size: 16);
            user.PasswordHashed = CryptographyHelper.Hash(inputPassword, user.PasswordSalt);

            var login = new LoginDTO
            {
                UserName = user.Username,
                Password = inputPassword
            };

            repositoryMock.Setup(x => x.FirstOrDefaultAsync<User>(It.IsAny<Expression<Func<User, bool>>>(), false))
                .ReturnsAsync(user);

            // Assert
            var response = await service.LoginAsync(login);

            // Assert
            response.Should().NotBeNull();
            response.UserId.Should().Be(user.Id);
            response.Token.Should().NotBeNull();
            response.Role.Should().Be(user.Role.Name);
        }
    }
}
