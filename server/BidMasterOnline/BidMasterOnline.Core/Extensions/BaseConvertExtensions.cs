using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;

namespace BidMasterOnline.Core.Extensions
{
    public static class BaseConvertExtensions
    {
        public static PaginatedList<T> ToPaginatedList<T>(this ListModel<T> domainList)
            => new PaginatedList<T>
            {
                Items = domainList.Items,
                Pagination = new Pagination
                {
                    PageSize = domainList.PageSize,
                    CurrentPage = domainList.CurrentPage,
                    TotalCount = domainList.TotalCount,
                    TotalPages = domainList.TotalPages,
                }
            };

        public static PaginatedList<TDTO> ToPaginatedList<TEntity, TDTO>(this ListModel<TEntity> domainList,
            Func<TEntity, TDTO> convertExpression)
            => new PaginatedList<TDTO>
               {
                   Items = domainList.Items.Select(convertExpression).ToList(),
                   Pagination = new Pagination
                   {
                       PageSize = domainList.PageSize,
                       CurrentPage = domainList.CurrentPage,
                       TotalCount = domainList.TotalCount,
                       TotalPages = domainList.TotalPages,
                   }
               };

        public static UserSummaryDTO ToSummaryDTO(this User entity)
            => new()
            {
                Id = entity.Id,
                Email = entity.Email,
                Username = entity.Username
            };
    }
}
