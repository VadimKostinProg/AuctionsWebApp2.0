using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Users.Service.API.ServiceContracts.Moderator;

namespace Users.Service.API.BackgroundJobs
{
    public class UnblockingUsersBackgroundJob : IJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRepository _repository;
        private readonly ILogger<UnblockingUsersBackgroundJob> _logger;

        public UnblockingUsersBackgroundJob(IServiceScopeFactory scopeFactory,
            IRepository repository,
            ILogger<UnblockingUsersBackgroundJob> logger)
        {
            _scopeFactory = scopeFactory;
            _repository = repository;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("UnblockingUsersBackgroundJob: started work...");

            List<long> blockedUserIds = await _repository
                .GetFiltered<User>(e => e.Status == UserStatus.Blocked && e.UnblockDateTime.HasValue && e.UnblockDateTime <= DateTime.UtcNow)
                .Select(user => user.Id)
                .ToListAsync();

            string message = $"UnblockingUsersBackgroundJob: " +
                $"fetched {blockedUserIds.Count} blocked users.";

            if (blockedUserIds.Count > 0)
            {
                message += " Started chenging the status of users...";
            }
            else
            {
                message += " Skiped changing users status for now.";
            }

            _logger.LogInformation(message);

            if (blockedUserIds.Count > 0)
            {
                int succeededUsers = 0;
                int failedUsers = 0;

                await Parallel.ForEachAsync(blockedUserIds, async (userId, token) =>
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();

                    IUsersService service = scope.ServiceProvider.GetRequiredService<IUsersService>();

                    ServiceResult result = await service.UnblockUserAsync(userId, token);

                    if (result.IsSuccessfull)
                        Interlocked.Increment(ref succeededUsers);
                    else
                        Interlocked.Increment(ref failedUsers);
                });

                _logger.LogInformation($"UnblockingUsersBackgroundJob: successfully changed status for " +
                    $"{succeededUsers} of {blockedUserIds.Count} blocked users. " +
                    $"Failed {failedUsers} of {blockedUserIds.Count} blocked users.");
            }
        }
    }
}
