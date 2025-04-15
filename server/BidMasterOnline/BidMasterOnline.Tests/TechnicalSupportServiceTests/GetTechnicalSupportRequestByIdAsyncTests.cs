using BidMasterOnline.Application.DTO;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BidMasterOnline.Tests.TechnicalSupportRequestsServiceTests
{
    /// <summary>
    /// Class for ITechnicalSupportRequestsService.GetTechnicalSupportRequestByIdAsync method unit tests.
    /// </summary>
    public class GetTechnicalSupportRequestByIdAsyncTests : TechnicalSupportRequestsServiceTestsBase
    {
        [Fact]
        public async Task GetTechnicalSupportRequestByAsyncId_IncorrectId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.GetByIdAsync<TechnicalSupportRequest>(It.IsAny<Guid>(), false))
                .ReturnsAsync(null as TechnicalSupportRequest);

            // Assert
            var action = async () =>
            {
                // Act
                var technicalSupportRequest = await service.GetTechnicalSupportRequestByIdAsync(idToPass);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task GetTechnicalSupportRequestByAsyncId_ValidId_ReturnsValidObject()
        {
            // Arrange
            var user = GetTestUser();

            var technicalSupportRequest = GetTestTechnicalSupportRequest(user);

            var idToPass = technicalSupportRequest.Id;

            repositoryMock.Setup(x => x.GetByIdAsync<TechnicalSupportRequest>(It.IsAny<Guid>(), false))
                .ReturnsAsync(technicalSupportRequest);

            var expectedTechnicalSupportRequest = new TechnicalSupportRequestDTO
            {
                Id = technicalSupportRequest.Id,
                UserId = technicalSupportRequest.UserId,
                Username = technicalSupportRequest.User.Username,
                RequestText = technicalSupportRequest.RequestText,
                DateAndTime = technicalSupportRequest.DateAndTime,
                IsHandled = technicalSupportRequest.IsHandled
            };

            // Act
            var actualTechnicalSupportRequest = await service.GetTechnicalSupportRequestByIdAsync(idToPass);

            // Assert
            actualTechnicalSupportRequest.Should().NotBeNull();
            actualTechnicalSupportRequest.Should().BeEquivalentTo(expectedTechnicalSupportRequest);
        }
    }
}
