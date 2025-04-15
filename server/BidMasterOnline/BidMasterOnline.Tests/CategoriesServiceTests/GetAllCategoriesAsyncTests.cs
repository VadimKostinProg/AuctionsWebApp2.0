using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BidMasterOnline.Tests.CategoriesServiceTests
{
    /// <summary>
    /// Class for ICategoryService.GetAllCategoriesAsync method unit tests.
    /// </summary>
    public class GetAllCategoriesAsyncTests : CategoriesServiceTestsBase
    {
        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsCategoriesOrderedByName()
        {
            // Arrange
            var categories = this.GetTestCategories().OrderBy(x => x.Name).ToList();

            var expectedCategories = categories.Select(x => mapper.Map<CategoryDTO>(x)).ToList();

            repositoryMock.Setup(x => x.GetAsync<Category>(It.IsAny<ISpecification<Category>>(), false))
                .ReturnsAsync(categories);

            // Act
            var actualCategories = await service.GetAllCategoriesAsync();

            // Assert
            actualCategories.Should().NotBeNull();
            actualCategories.Should().BeEquivalentTo(expectedCategories);
        }
    }
}
