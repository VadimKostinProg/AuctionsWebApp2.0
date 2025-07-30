using BidMasterOnline.Core.ServiceContracts;
using Microsoft.EntityFrameworkCore.Storage;

namespace BidMasterOnline.Infrastructure.Services
{
    public class TransactionsService : ITransactionsService
    {
        private readonly ApplicationContext _context;

        public TransactionsService(ApplicationContext context)
        {
            _context = context;
        }

        public IDbContextTransaction BeginTransaction()
            => _context.Database.BeginTransaction();
    }
}
