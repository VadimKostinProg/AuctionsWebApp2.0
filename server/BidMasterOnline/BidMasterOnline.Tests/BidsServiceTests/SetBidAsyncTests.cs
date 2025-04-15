using AutoFixture;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Enums;
using BidMasterOnline.Application.Exceptions;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.BidsServiceTests
{
    /// <summary>
    /// Class for IBidsService.SetBidAsync method unit tests.
    /// </summary>
    public class SetBidAsyncTests : BidsServiceTestsBase
    {
        [Fact]
        public async Task SetBidAsync_NullPassed_ThrowsArgumentNullException()
        {
            // Arrange
            SetBidDTO bid = null;

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetBidAsync(bid);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task SetBidAsync_UserIsBlocked_ThrowsForbiddenException()
        {
            // Arrange
            var authorizedUser = this.GetTestUser(Application.Enums.UserStatus.Blocked);
            var authorizedUserDTO = new UserDTO
            {
                Id = authorizedUser.Id,
                Username = authorizedUser.Username,
                FullName = authorizedUser.FullName,
                Email = authorizedUser.Email,
                Status = Enum.Parse<Application.Enums.UserStatus>(authorizedUser.UserStatus.Name),
                ImageUrl = string.Empty
            };

            var auction = this.GetTestAuction();
            auction.FinishDateTime = DateTime.UtcNow.AddDays(1);

            repositoryMock.Setup(x => x.GetByIdAsync<Auction>(auction.Id, false))
                .ReturnsAsync(auction);
            authServiceMock.Setup(x => x.GetAuthenticatedUserAsync())
                .ReturnsAsync(authorizedUserDTO);


            var bid = fixture.Build<SetBidDTO>()
                .With(x => x.AuctionId, auction.Id)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetBidAsync(bid);
            };

            await action.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task SetBidAsync_IncorrectAuctionId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var bid = fixture.Create<SetBidDTO>();

            var authorizedUser = this.GetTestUser();
            var authorizedUserDTO = new UserDTO
            {
                Id = authorizedUser.Id,
                Username = authorizedUser.Username,
                FullName = authorizedUser.FullName,
                Email = authorizedUser.Email,
                Status = Enum.Parse<Application.Enums.UserStatus>(authorizedUser.UserStatus.Name),
                ImageUrl = string.Empty
            };

            repositoryMock.Setup(x => x.GetByIdAsync<Auction>(It.IsAny<Guid>(), false))
                .ReturnsAsync(null as Auction);
            authServiceMock.Setup(x => x.GetAuthenticatedUserAsync())
                .ReturnsAsync(authorizedUserDTO);

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetBidAsync(bid);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task SetBidAsync_AuctionFinished_ThrowsInvalidOperationException()
        {
            // Arrange
            var authorizedUser = this.GetTestUser();
            var authorizedUserDTO = new UserDTO
            {
                Id = authorizedUser.Id,
                Username = authorizedUser.Username,
                FullName = authorizedUser.FullName,
                Email = authorizedUser.Email,
                Status = Enum.Parse<Application.Enums.UserStatus>(authorizedUser.UserStatus.Name),
                ImageUrl = string.Empty
            };

            var auction = this.GetTestAuction(Application.Enums.AuctionStatus.Finished);
            auction.FinishDateTime = DateTime.UtcNow.AddDays(-1);

            repositoryMock.Setup(x => x.GetByIdAsync<Auction>(auction.Id, false))
                .ReturnsAsync(auction);
            authServiceMock.Setup(x => x.GetAuthenticatedUserAsync())
                .ReturnsAsync(authorizedUserDTO);

            var bid = fixture.Build<SetBidDTO>()
                .With(x => x.AuctionId, auction.Id)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetBidAsync(bid);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task SetBidAsync_AuctionCanceled_ThrowsInvalidOperationException()
        {
            // Arrange
            var authorizedUser = this.GetTestUser();
            var authorizedUserDTO = new UserDTO
            {
                Id = authorizedUser.Id,
                Username = authorizedUser.Username,
                FullName = authorizedUser.FullName,
                Email = authorizedUser.Email,
                Status = Enum.Parse<Application.Enums.UserStatus>(authorizedUser.UserStatus.Name),
                ImageUrl = string.Empty
            };

            var auction = this.GetTestAuction(Application.Enums.AuctionStatus.Canceled);
            auction.FinishDateTime = DateTime.UtcNow.AddDays(1);

            repositoryMock.Setup(x => x.GetByIdAsync<Auction>(auction.Id, false))
                .ReturnsAsync(auction);
            authServiceMock.Setup(x => x.GetAuthenticatedUserAsync())
                .ReturnsAsync(authorizedUserDTO);

            var bid = fixture.Build<SetBidDTO>()
                .With(x => x.AuctionId, auction.Id)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetBidAsync(bid);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task SetBidAsync_AuctionistTryesToSetBid_ThrowsInvalidOperationException()
        {
            // Arrange
            var authorizedUser = this.GetTestUser();
            var authorizedUserDTO = new UserDTO
            {
                Id = authorizedUser.Id,
                Username = authorizedUser.Username,
                FullName = authorizedUser.FullName,
                Email = authorizedUser.Email,
                Status = Enum.Parse<Application.Enums.UserStatus>(authorizedUser.UserStatus.Name),
                ImageUrl = string.Empty
            };

            var auction = this.GetTestAuction(Application.Enums.AuctionStatus.Active);
            auction.AuctionistId = authorizedUser.Id;
            auction.FinishDateTime = DateTime.UtcNow.AddDays(1);

            repositoryMock.Setup(x => x.GetByIdAsync<Auction>(auction.Id, false))
                .ReturnsAsync(auction);
            authServiceMock.Setup(x => x.GetAuthenticatedUserAsync())
                .ReturnsAsync(authorizedUserDTO);

            var bid = fixture.Build<SetBidDTO>()
                .With(x => x.AuctionId, auction.Id)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetBidAsync(bid);
            };

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task SetBidAsync_AmountLessThanLastBid_ThrowsArgumentExcpetion()
        {
            // Arrange
            var user = this.GetTestUser();

            var authorizedUser = this.GetTestUser();
            var authorizedUserDTO = new UserDTO
            {
                Id = authorizedUser.Id,
                Username = authorizedUser.Username,
                FullName = authorizedUser.FullName,
                Email = authorizedUser.Email,
                Status = Enum.Parse<Application.Enums.UserStatus>(authorizedUser.UserStatus.Name),
                ImageUrl = string.Empty
            };

            var auction = this.GetTestAuction();
            auction.FinishDateTime = DateTime.UtcNow.AddDays(1);

            var bid = this.GetTestBid(auction, user);

            auction.Bids = new List<Bid> { bid };

            repositoryMock.Setup(x => x.GetByIdAsync<Auction>(auction.Id, false))
                .ReturnsAsync(auction);
            authServiceMock.Setup(x => x.GetAuthenticatedUserAsync())
                .ReturnsAsync(authorizedUserDTO);

            var bidToSet = fixture.Build<SetBidDTO>()
                .With(x => x.AuctionId, auction.Id)
                .With(x => x.Amount, bid.Amount - 1)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetBidAsync(bidToSet);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task SetBidAsync_FirstBidLessThanStartPrice_ThrowsArgumentExcpetion()
        {
            // Arrange
            var user = this.GetTestUser();

            var authorizedUser = this.GetTestUser();
            var authorizedUserDTO = new UserDTO
            {
                Id = authorizedUser.Id,
                Username = authorizedUser.Username,
                FullName = authorizedUser.FullName,
                Email = authorizedUser.Email,
                Status = Enum.Parse<Application.Enums.UserStatus>(authorizedUser.UserStatus.Name),
                ImageUrl = string.Empty
            };

            var auction = this.GetTestAuction();
            auction.FinishDateTime = DateTime.UtcNow.AddDays(1);

            auction.Bids = new List<Bid>();

            repositoryMock.Setup(x => x.GetByIdAsync<Auction>(auction.Id, false))
                .ReturnsAsync(auction);
            authServiceMock.Setup(x => x.GetAuthenticatedUserAsync())
                .ReturnsAsync(authorizedUserDTO);

            var bidToSet = fixture.Build<SetBidDTO>()
                .With(x => x.AuctionId, auction.Id)
                .With(x => x.Amount, auction.StartPrice - 1)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetBidAsync(bidToSet);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task SetBidAsync_FinishTypeStatic_SuccessfullSettingNewBid()
        {
            // Arrange
            var user = this.GetTestUser();

            var authorizedUser = this.GetTestUser();
            var authorizedUserDTO = new UserDTO
            {
                Id = authorizedUser.Id,
                Username = authorizedUser.Username,
                FullName = authorizedUser.FullName,
                Email = authorizedUser.Email,
                Status = Enum.Parse<Application.Enums.UserStatus>(authorizedUser.UserStatus.Name),
                ImageUrl = string.Empty
            };

            var auction = this.GetTestAuction();
            auction.FinishDateTime = DateTime.UtcNow.AddDays(1);

            auction.Bids = new List<Bid>();

            repositoryMock.Setup(x => x.GetByIdAsync<Auction>(auction.Id, false))
                .ReturnsAsync(auction);
            repositoryMock.Setup(x => x.AddAsync<Bid>(It.IsAny<Bid>()))
                .Returns(Task.CompletedTask);
            authServiceMock.Setup(x => x.GetAuthenticatedUserAsync())
                .ReturnsAsync(authorizedUserDTO);

            var bidToSet = fixture.Build<SetBidDTO>()
                .With(x => x.AuctionId, auction.Id)
                .With(x => x.Amount, auction.StartPrice + 10)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetBidAsync(bidToSet);
            };

            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task SetBidAsync_FinishTypeIncreasing_SuccessfullSettingNewBid()
        {
            // Arrange
            var user = this.GetTestUser();

            var authorizedUser = this.GetTestUser();
            var authorizedUserDTO = new UserDTO
            {
                Id = authorizedUser.Id,
                Username = authorizedUser.Username,
                FullName = authorizedUser.FullName,
                Email = authorizedUser.Email,
                Status = Enum.Parse<Application.Enums.UserStatus>(authorizedUser.UserStatus.Name),
                ImageUrl = string.Empty
            };

            var auction = this.GetTestAuction(auctionFinishType: Application.Enums.AuctionFinishType.IncreasingFinishTime);
            auction.FinishDateTime = DateTime.UtcNow.AddDays(1);

            auction.Bids = new List<Bid>();

            repositoryMock.Setup(x => x.GetByIdAsync<Auction>(auction.Id, false))
                .ReturnsAsync(auction);
            repositoryMock.Setup(x => x.AddAsync<Bid>(It.IsAny<Bid>()))
                .Returns(Task.CompletedTask);
            repositoryMock.Setup(x => x.UpdateAsync<Auction>(It.IsAny<Auction>()))
                .Returns(Task.CompletedTask);
            authServiceMock.Setup(x => x.GetAuthenticatedUserAsync())
                .ReturnsAsync(authorizedUserDTO);

            var bidToSet = fixture.Build<SetBidDTO>()
                .With(x => x.AuctionId, auction.Id)
                .With(x => x.Amount, auction.StartPrice + 10)
                .Create();

            // Assert
            var action = async () =>
            {
                // Act
                await service.SetBidAsync(bidToSet);
            };

            await action.Should().NotThrowAsync();
        }
    }
}
