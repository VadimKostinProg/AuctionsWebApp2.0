using AutoFixture;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.CategoriesServiceTests
{
    /// <summary>
    /// Class for ICategoryService.UpdateCategoryAsync method unit tests.
    /// </summary>
    public class UpdateCategoryAsyncTests : CategoriesServiceTestsBase
    {
        [Fact]
        public async Task UpdateNewCategoryAsync_NullPassed_ThrowsArgumentNullException()
        {
            // Arrange
            UpdateCategoryDTO categoryToUpdate = null;

            // Assert
            var action = async () =>
            {
                // Act
                await service.UpdateCategoryAsync(categoryToUpdate);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateNewCategoryAsync_IncorrectId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var categoryToUpdate = fixture.Create<UpdateCategoryDTO>();

            repositoryMock.Setup(x => x.GetByIdAsync<Category>(It.IsAny<Guid>(), false))
                .ReturnsAsync(null as Category);

            // Assert
            var action = async () =>
            {
                // Act
                await service.UpdateCategoryAsync(categoryToUpdate);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UpdateNewCategoryAsync_NameIsBlank_ThrowsArgumentNullException()
        {
            // Arrange
            var categoryToUpdate = fixture.Build<UpdateCategoryDTO>()
                .With(x => x.Name, string.Empty)
                .Create();

            var existantCategory = this.GetTestCategory();

            repositoryMock.Setup(x => x.GetByIdAsync<Category>(It.IsAny<Guid>(), false))
                .ReturnsAsync(existantCategory);

            // Assert
            var action = async () =>
            {
                // Act
                await service.UpdateCategoryAsync(categoryToUpdate);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateNewCategoryAsync_DescriptionIsBlank_ThrowsArgumentNullException()
        {
            // Arrange
            var categoryToUpdate = fixture.Build<UpdateCategoryDTO>()
                .With(x => x.Description, string.Empty)
                .Create();

            var existantCategory = this.GetTestCategory();

            repositoryMock.Setup(x => x.GetByIdAsync<Category>(It.IsAny<Guid>(), false))
                .ReturnsAsync(existantCategory);

            // Assert
            var action = async () =>
            {
                // Act
                await service.UpdateCategoryAsync(categoryToUpdate);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateNewCategoryAsync_NameAlreadyExists_ThrowsArgumentException()
        {
            // Arrange
            var categoryToUpdate = fixture.Create<UpdateCategoryDTO>();

            var existantCategory = this.GetTestCategory();

            repositoryMock.Setup(x => x.AnyAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(true);

            repositoryMock.Setup(x => x.GetByIdAsync<Category>(It.IsAny<Guid>(), false))
                .ReturnsAsync(existantCategory);

            // Assert
            var action = async () =>
            {
                // Act
                await service.UpdateCategoryAsync(categoryToUpdate);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdateNewCategoryAsync_ValidObject_SuccessfullCategoryCreation()
        {
            // Arrange
            var categoryToUpdate = fixture.Create<UpdateCategoryDTO>();

            var existantCategory = this.GetTestCategory();

            repositoryMock.Setup(x => x.AnyAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(false);
            repositoryMock.Setup(x => x.GetByIdAsync<Category>(It.IsAny<Guid>(), false))
                .ReturnsAsync(existantCategory);
            repositoryMock.Setup(x => x.UpdateAsync<Category>(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            // Assert
            var action = async () =>
            {
                // Act
                await service.UpdateCategoryAsync(categoryToUpdate);
            };

            await action.Should().NotThrowAsync();
        }
    }
}
