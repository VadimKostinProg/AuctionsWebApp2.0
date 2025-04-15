using BidMasterOnline.Application.DTO;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing categories of auction lots.
    /// </summary>
    public interface ICategoriesService
    {
        /// <summary>
        /// Gets all categories of auction lots.
        /// </summary>
        /// <returns>Collection IEnumerable of all categories.</returns>
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();

        /// <summary>
        /// Gets the list of categories with applyed specifications.
        /// </summary>
        /// <param name="specifications">Specifications for searchin, filtering, sorting and pagination to apply.</param>
        /// <returns>List of categories with applyed specifications.</returns>
        Task<ListModel<CategoryDTO>> GetCategoriesListAsync(SpecificationsDTO specifications);

        /// <summary>
        /// Gets the specified category by it`s identifier.
        /// </summary>
        /// <param name="id">Identifier of category to get.</param>
        /// <returns>Category with passed identifier.</returns>
        Task<CategoryDTO> GetCategoryByIdAsync(Guid id);

        /// <summary>
        /// Creates new category.
        /// </summary>
        /// <param name="category">Category to create.</param>
        Task CreateNewCategoryAsync(CreateCategoryDTO category);

        /// <summary>
        /// Updates existant category.
        /// </summary>
        /// <param name="category">Category to update.</param>
        Task UpdateCategoryAsync(UpdateCategoryDTO category);

        /// <summary>
        /// Deletes existant category.
        /// </summary>
        /// <param name="id">Identifier of category ot delete.</param>
        Task DeleteCategoryAsync(Guid id);
    }
}
