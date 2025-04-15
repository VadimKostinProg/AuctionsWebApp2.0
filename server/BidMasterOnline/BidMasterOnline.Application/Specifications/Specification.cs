using BidMasterOnline.Application.Enums;
using BidMasterOnline.Domain.Entities;
using System.Linq.Expressions;

namespace BidMasterOnline.Application.Specifications
{
    public class Specification<T> : SpecificationBase<T> where T : EntityBase
    {
        public Specification(Expression<Func<T, bool>> predicate = null,
                             Expression<Func<T, object>> sortBy = null,
                             SortDirection orderDirection = SortDirection.ASC)
        {
            this.Predicate = predicate;
            this.SortBy = sortBy;
            this.SortOrder = orderDirection;
        }

        public Specification(int take, int skip,
                             Expression<Func<T, bool>> predicate = null,
                             Expression<Func<T, object>> sortBy = null,
                             SortDirection orderDirection = SortDirection.ASC)
        {
            this.Predicate = predicate;
            this.SortBy = sortBy;
            this.SortOrder = orderDirection;

            this.IsPaginationEnabled = true;

            this.Take = take;
            this.Skip = skip;
        }
    }
}
