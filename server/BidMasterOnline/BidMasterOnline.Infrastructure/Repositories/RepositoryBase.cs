using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;
using BidMasterOnline.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public virtual async Task DeleteAsync<T>(Guid id) where T : EntityBase
        {
            await context.Set<T>().Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public Task DeleteAsync<T>(T entity) where T : EntityBase
        {
            context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public virtual Task DeleteManyAsync<T>(IEnumerable<T> entities) where T : EntityBase
        {
            context.Set<T>().RemoveRange(entities);
            return Task.CompletedTask;
        }

        public virtual async Task<T?> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression, bool disableTracking = false) where T : EntityBase
        {
            var query = context.Set<T>().AsQueryable();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(expression);
        }

        public virtual Task<IQueryable<T>> GetAllAsync<T>(bool disableTracking = false) where T : EntityBase
        {
            var query = context.Set<T>().AsQueryable();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return Task.FromResult(query);
        }

        public virtual Task<IQueryable<T>> GetAsync<T>(ISpecification<T> specification, bool disableTracking = false) where T : EntityBase
        {
            var query = context.Set<T>().AsQueryable();

            if (specification is not null)
            {
                query = query.ApplySpecifications(specification);
            }

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return Task.FromResult(query);
        }

        public virtual async Task<T?> GetByIdAsync<T>(Guid id, bool disableTracking = false) where T : EntityBase
        {
            var query = context.Set<T>().AsQueryable();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual Task<IQueryable<T>> GetFilteredAsync<T>(Expression<Func<T, bool>> predicate, bool disableTracking = false) where T : EntityBase
        {
            var query = context.Set<T>().Where(predicate);

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return Task.FromResult(query);
        }

        public Task<int> SaveChangesAsync()
            => context.SaveChangesAsync();

        public virtual Task UpdateAsync<T>(T entity) where T : EntityBase
        {
            context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}
