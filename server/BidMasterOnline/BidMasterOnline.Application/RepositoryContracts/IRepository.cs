using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;
using System.Linq.Expressions;

namespace BidMasterOnline.Application.RepositoryContracts
{
    /// <summary>
    /// Data access repository for EntityBase objects.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Method for count total amount of the entities of the passed type.
        /// </summary>
        /// <typeparam name="T">Type of entity to count amount.</typeparam>
        /// <returns>Amount of all entities of the passed type.</returns>
        Task<int> CountAsync<T>() where T : EntityBase;

        /// <summary>
        /// Method for count total amount of the entities of the passed type with applyed predicate.
        /// </summary>
        /// <typeparam name="T">Type of entity to count amount.</typeparam>
        /// <param name="predicate">Predicate for filtering entities.</param>
        /// <returns>Amount of all entities of the passed type with applyed predicate.</returns>
        Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : EntityBase;

        /// <summary>
        /// Method for reading all entities of the specific type from the data source.
        /// </summary>
        /// <typeparam name="T">Type of entities to read.</typeparam>
        /// <param name="disableTracking">Flag for enabling a tracking.</param>
        /// <returns>Queryable collection of all entities.</returns>
        Task<IQueryable<T>> GetAllAsync<T>(bool disableTracking = false) where T : EntityBase;

        /// <summary>
        /// Method for reading entities of the specific type for the data source with the applyed specifications.
        /// </summary>
        /// <typeparam name="T">Type of entities to read.</typeparam>
        /// <param name="specification">Specifications for filtering, sorting and pagination to apply.</param>
        /// <param name="disableTracking">Flag for enabling a tracking.</param>
        /// <returns>Queryable collection of entities with applyed specifications.</returns>
        Task<IQueryable<T>> GetAsync<T>(ISpecification<T> specification, bool disableTracking = false) where T : EntityBase;

        /// <summary>
        /// Method for reading entities of the specific type from the data source filtered by predicate.
        /// </summary>
        /// <typeparam name="T">Type of entities to read.</typeparam>
        /// <param name="predicate">Expression to filter entities.</param>
        /// <param name="disableTracking">Flag for enabling a tracking.</param>
        /// <returns>Queryable collection of filtered entities.</returns>
        Task<IQueryable<T>> GetFilteredAsync<T>(Expression<Func<T, bool>> predicate,
                                                 bool disableTracking = false)
                                                 where T : EntityBase;

        /// <summary>
        /// Method for searching the entity of the specific type from the data source by its id.
        /// </summary>
        /// <typeparam name="T">Type of entity to search.</typeparam>
        /// <param name="id">Guid of the entity.</param>
        /// <param name="disableTracking">Flag for enabling a tracking.</param>
        /// <returns>Found entity of the specific type, null - if entity with passed id was not found.</returns>
        Task<T?> GetByIdAsync<T>(Guid id, bool disableTracking = false) where T : EntityBase;

        /// <summary>
        /// Method for searching the entity of the specific type from the data source by specific criteria.
        /// </summary>
        /// <typeparam name="T">Type of entity to search.</typeparam>
        /// <param name="expression">Expression to search.</param>
        /// <param name="disableTracking">Flag for enabling a tracking.</param>
        /// <returns>Found entity of the specific type, null - if any entity with passed criteria was not found.</returns>
        Task<T?> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression, bool disableTracking = false) where T : EntityBase;

        /// <summary>
        /// Method for checking if the data source contains any row of the passed entity.
        /// </summary>
        /// <typeparam name="T">Type of entity to check.</typeparam>
        /// <returns>True - if the data source contains at least one row of the passed entity, 
        /// otherwise - false.</returns>
        Task<bool> AnyAsync<T>() where T : EntityBase;

        /// <summary>
        /// Method for checking if the data source contains the entity that fits passed expression.
        /// </summary>
        /// <typeparam name="T">Type of entity to check.</typeparam>
        /// <param name="expression">Expression to check.</param>
        /// <returns>True - if the data source contains the entity that fits passed expression, 
        /// otherwise - false.</returns>
        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> expression) where T : EntityBase;

        /// <summary>
        /// Method for inserting new entity into the data source.
        /// </summary>
        /// <typeparam name="T">Type of entity to insert.</typeparam>
        /// <param name="entity">Entity to insert.</param>
        Task AddAsync<T>(T entity) where T : EntityBase;

        /// <summary>
        /// Method for updating existent entity in the data source.
        /// </summary>
        /// <typeparam name="T">Type of entity to update.</typeparam>
        /// <param name="entity">Entity to update.</param>
        Task UpdateAsync<T>(T entity) where T : EntityBase;

        /// <summary>
        /// Method for deletin existent entity form the data source.
        /// </summary>
        /// <typeparam name="T">Type of entity to delete</typeparam>
        /// <param name="entity">Entity to delete.</param>
        Task DeleteAsync<T>(T entity) where T : EntityBase;

        /// <summary>
        /// Method for deleting existent entity form the data source by its identifier.
        /// </summary>
        /// <typeparam name="T">Type of entity to delete.</typeparam>
        /// <param name="id">Guid of entity to delete.</param>
        Task DeleteAsync<T>(Guid id) where T : EntityBase;

        /// <summary>
        /// Method for deleting the collection of the entities.
        /// </summary>
        /// <typeparam name="T">Type of entity to delete.</typeparam>
        /// <param name="entities">Collection of the entities to delete.</param>
        Task DeleteManyAsync<T>(IEnumerable<T> entities) where T : EntityBase;

        /// <summary>
        /// Saves all changes tracked by the data context.
        /// </summary>
        /// <returns>Rows number affected.</returns>
        Task<int> SaveChangesAsync();
    }
}
