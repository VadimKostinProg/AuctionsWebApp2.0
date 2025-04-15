using BidMasterOnline.Application.Enums;
using BidMasterOnline.Domain.Entities;
using System.Linq.Expressions;

namespace BidMasterOnline.Application.Specifications
{
    public class SpecificationBuilder<T> where T : EntityBase
    {
        private readonly List<Expression<Func<T, bool>>> _filters = new();
        private Expression<Func<T, object>> _sortBy;
        private SortDirection _sortOrder;
        private bool _isPaginationEnabled;
        private int _take;
        private int _skip;

        public SpecificationBuilder<T> With(Expression<Func<T, bool>> predicate)
        {
            this._filters.Add(predicate);

            return this;
        }

        public SpecificationBuilder<T> OrderBy(Expression<Func<T, object>> sortBy, SortDirection sortOrder)
        {
            this._sortBy = sortBy;
            this._sortOrder = sortOrder;

            return this;
        }

        public SpecificationBuilder<T> WithPagination(int pageSize, int pageNumber)
        {
            if (pageSize < 1)
                throw new ArgumentException("Page size must be at least 1.");

            if (pageNumber < 1)
                throw new ArgumentException("Page number must be at least 1.");

            this._isPaginationEnabled = true;

            this._take = pageSize;
            this._skip = (pageNumber - 1) * pageSize;

            return this;
        }

        public ISpecification<T> Build()
        {
            var specification = !this._isPaginationEnabled ?
                new Specification<T>(this.GetPredicate(), this._sortBy, this._sortOrder) :
                new Specification<T>(this._take, this._skip, this.GetPredicate(), this._sortBy, this._sortOrder);

            return specification;
        }

        private Expression<Func<T, bool>> GetPredicate()
        {
            if (!this._filters.Any())
                return null;

            Expression<Func<T, bool>> combinedExpression = this._filters.First();

            if (this._filters.Count > 1)
                for (int i = 0; i < this._filters.Count - 1; i++)
                    combinedExpression = this.Combine(_filters[i], _filters[i + 1]);

            return combinedExpression;
        }

        private Expression<Func<T, bool>> Combine(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");

            var body = Expression.AndAlso(
                    Expression.Invoke(left, param),
                    Expression.Invoke(right, param)
                );

            var lambda = Expression.Lambda<Func<T, bool>>(body, param);

            return lambda;
        }
    }
}
