using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace BidMasterOnline.Core.RepositoryContracts
{
    public interface IRepository
    {
        Task<int> CountAsync<T>() where T : EntityBase;

        Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : EntityBase;

        IQueryable<T> GetAll<T>(bool disableTracking = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null) where T : EntityBase;

        IQueryable<T> GetFiltered<T>(Expression<Func<T, bool>> predicate, 
            bool disableTracking = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null)
                                     where T : EntityBase;

        Task<ListModel<T>> GetFilteredAndPaginated<T>(ISpecification<T> specification, 
            bool disableTracking = false, 
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null)
            where T : EntityBase;

        T GetById<T>(long id, 
            bool disableTracking = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null) where T : EntityBase;

        Task<T> GetByIdAsync<T>(long id,
            bool disableTracking = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null) where T : EntityBase;

        Task<T?> GetFirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression, 
            bool disableTracking = false, 
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null) where T : EntityBase;

        Task<bool> AnyAsync<T>() where T : EntityBase;

        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> expression) where T : EntityBase;

        Task AddAsync<T>(T entity) where T : EntityBase;

        void Update<T>(T entity) where T : EntityBase;

        Task<int> UpdateManyAsync<T>(Expression<Func<T, bool>> predicate,
            Func<T, object> setProperyExpression,
            object value) 
            where T : EntityBase;

        void Delete<T>(T entity) where T : EntityBase;

        Task DeleteByIdAsync<T>(long id) where T : EntityBase;

        void DeleteMany<T>(IEnumerable<T> entities) where T : EntityBase;

        Task<int> SaveChangesAsync();
    }
}
