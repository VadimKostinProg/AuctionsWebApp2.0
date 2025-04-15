using AutoFixture;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BidMasterOnline.Tests.TechnicalSupportRequestsServiceTests
{
    /// <summary>
    /// Class for ITechnicalSupportRequestsService.SetTechnicalSupportRequestAsync method unit tests.
    /// </summary>
    public class SetTechnicalSupportRequestAsyncTests : TechnicalSupportRequestsServiceTestsBase
    {
        [Fact]
        public async Task SetTechnicalSupportRequestAsync_NullPassed_ThrowsArgumentNullException()
        {
            // Arrange
            SetTechnicalSupportRequestDTO request = null;

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetTechnicalSupportRequestAsync(request);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task SetTechnicalSupportRequestAsync_ValidObject_SucessfullySetsNewRequest()
        {
            // Arrange
            var request = fixture.Create<SetTechnicalSupportRequestDTO>();

            var user = GetTestUser();

            authServiceMock.Setup(x => x.GetAuthenticatedUserEntityAsync())
                .ReturnsAsync(user);
            repositoryMock.Setup(x => x.AddAsync(It.IsAny<TechnicalSupportRequest>()))
                .Returns(Task.CompletedTask);

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetTechnicalSupportRequestAsync(request);
            };

            await action.Should().NotThrowAsync();
        }
    }
}
