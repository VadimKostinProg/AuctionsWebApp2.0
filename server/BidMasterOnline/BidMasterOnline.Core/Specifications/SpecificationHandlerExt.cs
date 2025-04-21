using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Core.Specifications
{
    public static class SpecificationHandlerExt
    {
        public static IQueryable<T> ApplySpecifications<T>(this IQueryable<T> query, ISpecification<T> specification)
            where T : EntityBase
        {
            if (specification.Predicate is not null)
            {
                query = query.Where(specification.Predicate);
            }

            if (specification.SortBy is not null)
            {
                switch (specification.SortOrder)
                {
                    case Enums.SortDirection.ASC:
                        query = query.OrderBy(specification.SortBy);
                        break;
                    case Enums.SortDirection.DESC:
                        query = query.OrderByDescending(specification.SortBy);
                        break;
                }
            }

            int skip = (specification.PageNumber - 1) * specification.PageSize;
            int take = specification.PageSize;

            query = query.Skip(skip)
                .Take(take);

            return query;
        }
    }
}
