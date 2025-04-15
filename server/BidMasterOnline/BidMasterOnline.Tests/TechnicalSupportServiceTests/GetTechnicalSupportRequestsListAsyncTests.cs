using AutoFixture;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.TechnicalSupportRequestsServiceTests
{
    /// <summary>
    /// Class for ITechnicalSupportRequestsService.GetTechnicalSupportRequestsListAsync method unit tests.
    /// </summary>
    public class GetTechnicalSupportRequestsListAsyncTests : TechnicalSupportRequestsServiceTestsBase
    {
        [Fact]
        public async Task GetTechnicalSupportRequestsListAsync_SpecificationsAreNull_ThrowsArgumentNullExcpetion()
        {
            // Arrange
            TechnicalSupportRequestSpecificationsDTO specifications = null;

            // Assert
            var action = async () =>
            {
                // Act
                var list = await service.GetTechnicalSupportRequestsListAsync(specifications);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetTechnicalSupportRequestsListAsync_ValidSpecifications_RetunsList()
        {
            // Arrange
            var specifications = new TechnicalSupportRequestSpecificationsDTO
            {
                IsHandled = false,
                PageNumber = 1,
                PageSize = 10,
            };

            var user = GetTestUser();

            var technicalSupportRequests = GetTechnicalSupportRequests(user);

            repositoryMock.Setup(x => x.GetAsync<TechnicalSupportRequest>(It.IsAny<ISpecification<TechnicalSupportRequest>>(), false))
                .ReturnsAsync(technicalSupportRequests);
            repositoryMock.Setup(x => x.CountAsync<TechnicalSupportRequest>(It.IsAny<Expression<Func<TechnicalSupportRequest, bool>>>()))
                .ReturnsAsync(10);
            // Act
            var list = await service.GetTechnicalSupportRequestsListAsync(specifications);

            // Assert
            list.Should().NotBeNull();
            list.List.Should().HaveCount(technicalSupportRequests.Count());
            list.TotalPages.Should().Be(1);
        }
    }
}
