using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BidMasterOnline.Tests.TechnicalSupportRequestsServiceTests
{
    /// <summary>
    /// Class for ITechnicalSupportRequestsService.HandleTechnicalSupportRequestAsync method unit tests.
    /// </summary>
    public class HandleTechnicalSupportRequestAsyncTests : TechnicalSupportRequestsServiceTestsBase
    {
        [Fact]
        public async Task HandleTechnicalSupportRequestAsync_IncorrectId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.GetByIdAsync<TechnicalSupportRequest>(It.IsAny<Guid>(), false))
                .ReturnsAsync(null as TechnicalSupportRequest);

            // Assert
            var action = async () =>
            {
                // Act
                await service.HandleTechnicalSupportRequestAsync(idToPass);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task HandleTechnicalSupportRequestAsync_TechnicalSupportRequestIsHandled_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = GetTestUser();

            var technicalSupportRequest = GetTestTechnicalSupportRequest(user, isHandled: true);

            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.GetByIdAsync<TechnicalSupportRequest>(It.IsAny<Guid>(), false))
                .ReturnsAsync(technicalSupportRequest);

            // Assert
            var action = async () =>
            {
                // Act
                await service.HandleTechnicalSupportRequestAsync(idToPass);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task HandleTechnicalSupportRequestAsync_ValidRequest_SuccessfullyHandlesTechnicalSupportRequest()
        {
            // Arrange
            var user = GetTestUser();

            var technicalSupportRequest = GetTestTechnicalSupportRequest(user);

            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.GetByIdAsync<TechnicalSupportRequest>(It.IsAny<Guid>(), false))
                .ReturnsAsync(technicalSupportRequest);
            repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TechnicalSupportRequest>()))
                .Returns(Task.CompletedTask);

            // Assert
            var action = async () =>
            {
                // Act
                await service.HandleTechnicalSupportRequestAsync(idToPass);
            };

            await action.Should().NotThrowAsync();
        }
    }
}
