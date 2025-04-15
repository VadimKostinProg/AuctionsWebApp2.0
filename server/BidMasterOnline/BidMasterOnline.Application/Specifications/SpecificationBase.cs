using BidMasterOnline.Application.Enums;
using BidMasterOnline.Domain.Entities;
using System.Linq.Expressions;

namespace BidMasterOnline.Application.Specifications
{
    public abstract class SpecificationBase<T> : ISpecification<T> where T : EntityBase
    {
        public virtual Expression<Func<T, bool>> Predicate { get; set; }
        public virtual Expression<Func<T, object>> SortBy { get; set; }
        public virtual SortDirection SortOrder { get; set; }
        public virtual bool IsPaginationEnabled { get; set; }
        public virtual int Skip { get; set; }
        public virtual int Take { get; set; }
    }
}
