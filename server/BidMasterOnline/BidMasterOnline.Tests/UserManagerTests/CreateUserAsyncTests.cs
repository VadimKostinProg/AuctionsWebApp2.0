using AutoFixture;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.UserManagerTests
{
    /// <summary>
    /// Class for IUserManager.CreateUserAsync method unit tests.
    /// </summary>
    public class CreateUserAsyncTests : UserManagerTestsBase
    {
        [Fact]
        public async Task CreateUserAsync_NullPassed_ThrowsArgumentNullException()
        {
            // Arrange
            CreateUserDTO user = null;

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateUserAsync(user, Application.Enums.UserRole.Customer);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateUserAsync_UserNameIsBlank_ThrowsArgumentNullException()
        {
            // Arrange
            CreateUserDTO user = fixture.Build<CreateUserDTO>()
                .With(x => x.Username, string.Empty)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateUserAsync(user, Application.Enums.UserRole.Customer);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateUserAsync_FullNameIsBlank_ThrowsArgumentNullException()
        {
            // Arrange
            CreateUserDTO user = fixture.Build<CreateUserDTO>()
                .With(x => x.FullName, string.Empty)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateUserAsync(user, Application.Enums.UserRole.Customer);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateUserAsync_EmailIsBlank_ThrowsArgumentNullException()
        {
            // Arrange
            CreateUserDTO user = fixture.Build<CreateUserDTO>()
                .With(x => x.Email, string.Empty)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateUserAsync(user, Application.Enums.UserRole.Customer);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateUserAsync_PasswordIsBlank_ThrowsArgumentNullException()
        {
            // Arrange
            CreateUserDTO user = fixture.Build<CreateUserDTO>()
                .With(x => x.Password, string.Empty)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateUserAsync(user, Application.Enums.UserRole.Customer);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateUserAsync_UsernameAlreadyExists_ThrowsArgumentException()
        {
            // Arrange
            CreateUserDTO user = fixture.Build<CreateUserDTO>()
                .With(x => x.Password, string.Empty)
                .Create();

            repositoryMock.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(true);

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateUserAsync(user, Application.Enums.UserRole.Customer);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateUserAsync_ValidObject_SuccessfullUserCreation()
        {
            // Arrange
            CreateUserDTO user = fixture.Create<CreateUserDTO>();

            var role = new Role()
            {
                Name = "Customer"
            };

            var status = new UserStatus()
            {
                Name = "Active"
            };

            repositoryMock.Setup(x => x.FirstOrDefaultAsync<Role>(It.IsAny<Expression<Func<Role, bool>>>(), false))
                .ReturnsAsync(role);
            repositoryMock.Setup(x => x.FirstOrDefaultAsync<UserStatus>(It.IsAny<Expression<Func<UserStatus, bool>>>(), false))
                .ReturnsAsync(status);
            repositoryMock.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(false);
            repositoryMock.Setup(x => x.AddAsync<User>(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateUserAsync(user, Application.Enums.UserRole.Customer);
            };

            await action.Should().NotThrowAsync();
        }
    }
}
