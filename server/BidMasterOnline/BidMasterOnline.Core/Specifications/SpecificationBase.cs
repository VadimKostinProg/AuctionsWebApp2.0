using BidMasterOnline.Core.Enums;
using BidMasterOnline.Domain.Entities;
using System.Linq.Expressions;

namespace BidMasterOnline.Core.Specifications
{
    public abstract class SpecificationBase<T> : ISpecification<T> where T : EntityBase
    {
        public virtual Expression<Func<T, bool>> Predicate { get; set; }
        public virtual Expression<Func<T, object>> SortBy { get; set; }
        public virtual SortDirection SortOrder { get; set; }
        public virtual int PageNumber { get; set; }
        public virtual int PageSize { get; set; }
    }
}
