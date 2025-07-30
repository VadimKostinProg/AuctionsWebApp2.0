using BidMasterOnline.Core.Enums;
using BidMasterOnline.Domain.Models.Entities;
using System.Linq.Expressions;

namespace BidMasterOnline.Core.Specifications
{
    public class Specification<T> : SpecificationBase<T> where T : EntityBase
    {
        public Specification(Expression<Func<T, bool>> predicate = null,
                             Expression<Func<T, object>> sortBy = null,
                             SortDirection orderDirection = SortDirection.ASC,
                             int pageNumber = 1,
                             int pageSize = 10)
        {
            this.Predicate = predicate;
            this.SortBy = sortBy;
            this.SortOrder = orderDirection;

            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }
    }
}
