using AutoFixture;
using BidMasterOnline.Application.Enums;
using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Tests
{
    /// <summary>
    /// Base test class for all unit test.
    /// </summary>
    public abstract class BidMasterOnlineTestsBase
    {
        protected readonly Fixture fixture = new();

        public Category GetTestCategory(bool isDeleted = false)
        {
            return fixture.Build<Category>()
                .With(x => x.IsDeleted, isDeleted)
                .Create();
        }

        public IEnumerable<Category> GetTestCategories(int count = 10, bool isDeleted = false)
        {
            for (int i = 0; i < count; i++)
            {
                yield return this.GetTestCategory(isDeleted);
            }
        }

        public Auction GetTestAuction(Application.Enums.AuctionStatus auctionStatus = Application.Enums.AuctionStatus.Active,
            Application.Enums.AuctionFinishType auctionFinishType = Application.Enums.AuctionFinishType.StaticFinishTime)
        {
            var status = new Domain.Entities.AuctionStatus()
            {
                Id = Guid.NewGuid(),
                Name = auctionStatus.ToString()
            };

            var finishType = new Domain.Entities.AuctionFinishType()
            {
                Id = Guid.NewGuid(),
                Name = auctionFinishType.ToString()
            };

            var auctioner = this.GetTestUser();

            return fixture.Build<Auction>()
                .With(x => x.AuctionistId, auctioner.Id)
                .With(x => x.Auctionist, auctioner)
                .With(x => x.StatusId, status.Id)
                .With(x => x.Status, status)
                .With(x => x.FinishTypeId, finishType.Id)
                .With(x => x.FinishType, finishType)
                .With(x => x.Bids, new List<Bid>())
                .Create();
        }

        public User GetTestUser(Application.Enums.UserStatus userStatus = Application.Enums.UserStatus.Active)
        {
            var status = new Domain.Entities.UserStatus()
            {
                Id = Guid.NewGuid(),
                Name = userStatus.ToString()
            };

            var role = new Role()
            {
                Id = Guid.NewGuid(),
                Name = UserRole.Customer.ToString()
            };

            return fixture.Build<User>()
                .With(x => x.RoleId, role.Id)
                .With(x => x.Role, role)
                .With(x => x.UserStatusId, status.Id)
                .With(x => x.UserStatus, status)
                .Create();
        }

        public IEnumerable<User> GetTestUsers(Application.Enums.UserStatus userStatus = Application.Enums.UserStatus.Active, int count = 10)
        {
            for (int i = 0; i < count; i++)
                yield return GetTestUser(userStatus);
        }

        public Bid GetTestBid(Auction auction, User bidder, bool isWinning = false)
        {
            return fixture.Build<Bid>()
                .With(x => x.AuctionId, auction.Id)
                .With(x => x.Auction, auction)
                .With(x => x.BidderId, bidder.Id)
                .With(x => x.Bidder, bidder)
                .With(x => x.IsWinning, isWinning)
                .Create();
        }

        public IEnumerable<Bid> GetTestBids(Auction auction, User bidder, int count = 10)
        {
            for (int i = 0; i < count; i++)
            {
                yield return this.GetTestBid(auction, bidder);
            }
        }

        public TechnicalSupportRequest GetTestTechnicalSupportRequest(User user, bool isHandled = false)
        {
            return new TechnicalSupportRequest
            {
                UserId = user.Id,
                DateAndTime = DateTime.UtcNow,
                RequestText = fixture.Create<string>(),
                IsHandled = isHandled,
                User = user,
            };
        }

        public IEnumerable<TechnicalSupportRequest> GetTechnicalSupportRequests(User user, int count = 10, bool isHandled = false)
        {
            for (int i = 0; i < count; i++)
                yield return GetTestTechnicalSupportRequest(user, isHandled);
        }
    }
}
