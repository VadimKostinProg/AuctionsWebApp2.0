using BidMasterOnline.Infrastructure.DatabaseContext;
using BidMasterOnline.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace BidMasterOnline.Infrastructure.Repositories
{
    public class RepositoryBase : IRepository
    {
        protected readonly ApplicationContext context;

        public RepositoryBase(ApplicationContext context)
        {
            this.context = context;
        }

        public virtual async Task AddAsync<T>(T entity) where T : EntityBase
        {
            await context.AddAsync(entity);
        }

        public Task<bool> AnyAsync<T>() where T : EntityBase
            => context.Set<T>().AnyAsync();

        public virtual Task<bool> AnyAsync<T>(Expression<Func<T, bool>> expression) where T : EntityBase
            => context.Set<T>().AnyAsync(expression);

        public virtual Task<int> CountAsync<T>() where T : EntityBase
            => context.Set<T>().CountAsync();

        public virtual Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : EntityBase
            => context.Set<T>().CountAsync(predicate);

        public virtual async Task DeleteByIdAsync<T>(long id) where T : EntityBase
        {
            await context.Set<T>().Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public void Delete<T>(T entity) where T : EntityBase
        {
            context.Set<T>().Remove(entity);
        }

        public virtual void DeleteMany<T>(IEnumerable<T> entities) where T : EntityBase
        {
            context.Set<T>().RemoveRange(entities);
        }

        public virtual async Task<T?> GetFirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression, 
            bool disableTracking = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null) where T : EntityBase
        {
            IQueryable<T> query = context.Set<T>().AsQueryable();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeQuery != null)
            {
                query = includeQuery(query);
            }

            return await query.FirstOrDefaultAsync(expression);
        }

        public virtual IQueryable<T> GetAll<T>(bool disableTracking = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null) where T : EntityBase
        {
            var query = context.Set<T>().AsQueryable();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeQuery != null)
            {
                query = includeQuery(query);
            }

            return query;
        }

        public virtual T GetById<T>(long id, 
            bool disableTracking = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null) where T : EntityBase
        {
            IQueryable<T> query = context.Set<T>().AsQueryable();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeQuery != null)
            {
                query = includeQuery(query);
            }

            return query.First(x => x.Id == id);

        }

        public virtual async Task<T> GetByIdAsync<T>(long id, 
            bool disableTracking = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null) where T : EntityBase
        {
            IQueryable<T> query = context.Set<T>().AsQueryable();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeQuery != null)
            {
                query = includeQuery(query);
            }

            return await query.FirstAsync(x => x.Id == id);
        }

        public virtual IQueryable<T> GetFiltered<T>(Expression<Func<T, bool>> predicate,
            bool disableTracking = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null)
            where T : EntityBase
        {
            var query = context.Set<T>().Where(predicate);

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includeQuery != null)
            {
                query = includeQuery(query);
            }

            return query;
        }

        public virtual void Update<T>(T entity) where T : EntityBase
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public Task<int> SaveChangesAsync()
            => context.SaveChangesAsync();

        public async Task<ListModel<T>> GetFilteredAndPaginated<T>(ISpecification<T> specification,
            bool disableTracking = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeQuery = null)
            where T : EntityBase
        {
            IQueryable<T> query = context.Set<T>();

            if (disableTracking)
            {
                query.AsNoTracking();
            }

            if (includeQuery != null)
            {
                query = includeQuery(query);
            }

            query = query.ApplySpecifications(specification);

            int totalCount = await this.CountAsync(specification.Predicate);
            int totalPages = (int)Math.Ceiling((double)totalCount / specification.PageSize);
            List<T> items = await query.ToListAsync();

            return new ListModel<T>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = specification.PageNumber,
                PageSize = specification.PageSize
            };
        }
    }
}
