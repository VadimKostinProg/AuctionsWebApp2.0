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
        private readonly IRepository _repository;
        private readonly IUserProfilesService _userProfilesService;

        public UnblockingUsersBackgroundJob(IRepository repository,
            IUserProfilesService userProfilesService)
        {
            _repository = repository;
            _userProfilesService = userProfilesService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            List<long> blockedUserIds = await _repository
                .GetFiltered<User>(e => e.Status == UserStatus.Blocked && e.UnblockDateTime.HasValue && e.UnblockDateTime >= DateTime.UtcNow)
                .Select(user => user.Id)
                .ToListAsync();

            await Parallel.ForEachAsync(blockedUserIds, async (userId, token) =>
            {
                await _userProfilesService.UnblockUserAsync(userId, token);
            });
        }
    }
}
