using AutoFixture;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.CategoriesServiceTests
{
    /// <summary>
    /// Class for ICategoryService.CreateNewCategoryAsync method unit tests.
    /// </summary>
    public class CreateNewCategoryAsyncTests : CategoriesServiceTestsBase
    {
        [Fact]
        public async Task CreateNewCategoryAsync_NullPassed_ThrowsArgumentNullException()
        {
            // Arrange
            CreateCategoryDTO categoryToCreate = null;

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateNewCategoryAsync(categoryToCreate);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateNewCategoryAsync_NameIsBlank_ThrowsArgumentNullException()
        {
            // Arrange
            var categoryToCreate = fixture.Build<CreateCategoryDTO>()
                .With(x => x.Name, string.Empty)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateNewCategoryAsync(categoryToCreate);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateNewCategoryAsync_DescriptionIsBlank_ThrowsArgumentNullException()
        {
            // Arrange
            var categoryToCreate = fixture.Build<CreateCategoryDTO>()
                .With(x => x.Description, string.Empty)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateNewCategoryAsync(categoryToCreate);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateNewCategoryAsync_NameAlreadyExists_ThrowsArgumentException()
        {
            // Arrange
            var categoryToCreate = fixture.Create<CreateCategoryDTO>();

            repositoryMock.Setup(x => x.AnyAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(true);

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateNewCategoryAsync(categoryToCreate);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateNewCategoryAsync_ValidObject_SuccessfullCategoryCreation()
        {
            // Arrange
            var categoryToCreate = fixture.Create<CreateCategoryDTO>();

            repositoryMock.Setup(x => x.AnyAsync<Category>(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync(false);
            repositoryMock.Setup(x => x.AddAsync<Category>(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            // Assert
            var action = async () =>
            {
                // Act
                await service.CreateNewCategoryAsync(categoryToCreate);
            };

            await action.Should().NotThrowAsync();
        }
    }
}
