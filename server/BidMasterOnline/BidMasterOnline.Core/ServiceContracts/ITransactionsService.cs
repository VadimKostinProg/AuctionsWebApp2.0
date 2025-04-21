using Microsoft.EntityFrameworkCore.Storage;

namespace BidMasterOnline.Core.ServiceContracts
{
    public interface ITransactionsService
    {
        IDbContextTransaction BeginTransaction();
    }
}
