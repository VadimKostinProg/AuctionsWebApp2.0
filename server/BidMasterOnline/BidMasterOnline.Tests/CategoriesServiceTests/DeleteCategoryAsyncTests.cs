using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.CategoriesServiceTests
{
    /// <summary>
    /// Class for ICategoryService.DeleteCategoryAsync method unit tests.
    /// </summary>
    public class DeleteCategoryAsyncTests : CategoriesServiceTestsBase
    {
        [Fact]
        public async Task DeleteCategoryAsync_IncorrectId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>(), false))
                .ReturnsAsync(null as Category);

            // Assert
            var action = async () =>
            {
                // Act
                await service.DeleteCategoryAsync(idToPass);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteCategoryAsync_ValidId_SuccessfullCategoryDeleting()
        {
            // Arrange
            var categoryToDelete = this.GetTestCategory();

            var idToPass = categoryToDelete.Id;

            repositoryMock.Setup(x => x.FirstOrDefaultAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>(), false))
                .ReturnsAsync(categoryToDelete);

            repositoryMock.Setup(x => x.UpdateAsync<Category>(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            // Assert
            var action = async () =>
            {
                // Act
                await service.DeleteCategoryAsync(idToPass);
            };

            await action.Should().NotThrowAsync();
        }
    }
}
