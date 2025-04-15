using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Enums;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.UserManagerTests
{
    /// <summary>
    /// Class for IUserManager.GetUsersListAsync method unit tests.
    /// </summary>
    public class GetUsersListAsyncTests : UserManagerTestsBase
    {
        [Fact]
        public async Task GetUsersListAsync_NullPassed_ThrowsArgumentNullException()
        {
            // Arrange
            UserSpecificationsDTO specifications = null;

            // Assert
            var action = async () =>
            {
                // Act
                var list = await service.GetCustomersListAsync(specifications);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetUserAsync_SpecificationsPassed_ReturnsUsersList()
        {
            // Arrange
            var users = this.GetTestUsers(count: 20).ToList();

            const string name = "test name";

            users[0].FullName = users[1].FullName = users[2].FullName = name;

            UserSpecificationsDTO specificationsDTO = new()
            {
                SearchTerm = "test",
                Role = Application.Enums.UserRole.Customer,
                Status = Application.Enums.UserStatus.Active,
                SortField = "FullName",
                PageNumber = 1,
                PageSize = 3
            };

            var expectedUsersList = users.Where(x => x.FullName == name)
                .OrderBy(x => x.FullName)
                .Take(3)
                .ToList();

            var expectedUsersDTO = expectedUsersList
                .Select(x => new UserDTO
                {
                    Id = x.Id,
                    Username = x.Username,
                    FullName = x.FullName,
                    Email = x.Email,
                    Role = Enum.Parse<Application.Enums.UserRole>(x.Role.Name),
                    ImageUrl = x.ImageUrl,
                    Status = Enum.Parse<Application.Enums.UserStatus>(x.UserStatus.Name)
                })
                .ToList();

            var expectedTotalPages = (long)Math.Ceiling((double)users.Count / specificationsDTO.PageSize);

            repositoryMock.Setup(x => x.GetAsync<User>(It.IsAny<ISpecification<User>>(), false))
                .ReturnsAsync(expectedUsersList);
            repositoryMock.Setup(x => x.CountAsync<User>(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(users.Count);

            // Act
            var usersList = await service.GetCustomersListAsync(specificationsDTO);

            // Assert
            usersList.Should().NotBeNull();
            usersList.List.Should().BeEquivalentTo(expectedUsersDTO);
            usersList.TotalPages.Should().Be(expectedTotalPages);
        }
    }
}
