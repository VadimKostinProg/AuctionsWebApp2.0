using AutoFixture;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace BidMasterOnline.Tests.BidsServiceTests
{
    /// <summary>
    /// Class for IBidsService.GetBidsListForUserAsync method unit tests.
    /// </summary>
    public class GetBidsListForUserAsyncTests : BidsServiceTestsBase
    {
        [Fact]
        public async Task GetBidsListForUserAsync_SpecificationsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            BidSpecificationsDTO specifications = null;

            // Assert
            var action = async () =>
            {
                // Act
                var bidsList = await service.GetBidsListForUserAsync(idToPass, specifications);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetBidsListForUserAsync_IncorrectUserId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(false);   

            var specifications = fixture.Create<BidSpecificationsDTO>();

            // Assert
            var action = async () =>
            {
                // Act
                var bidsList = await service.GetBidsListForUserAsync(idToPass, specifications);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Theory]
        [InlineData(5, 1)]
        [InlineData(3, 2)]
        [InlineData(4, 1)]
        [InlineData(2, 3)]
        [InlineData(1, 4)]
        public async Task GetBidsListForUserAsync_ValidId_ReturnsBidsList(int pageSize, int pageNumber)
        {
            // Arrange
            var auction = this.GetTestAuction();

            var user = this.GetTestUser();

            var allBids = this.GetTestBids(auction, user);

            var bids = allBids
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.DateAndTime)
                .ToList();

            repositoryMock.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(true);
            repositoryMock.Setup(x => x.GetAsync<Bid>(It.IsAny<ISpecification<Bid>>(), false))
                .ReturnsAsync(bids);
            repositoryMock.Setup(x => x.CountAsync<Bid>(It.IsAny<Expression<Func<Bid, bool>>>()))
                .ReturnsAsync(allBids.Count());

            var expectedBids = bids
                .Select(x => new BidDTO
                {
                    Id = x.Id,
                    BidderId = x.BidderId,
                    BidderUsername = x.Bidder.Username,
                    AuctionId = x.AuctionId,
                    AuctionName = x.Auction.Name,
                    DateAndTime = x.DateAndTime,
                    Amount = x.Amount
                })
                .ToList();

            var totalPages = (int)Math.Ceiling((double)allBids.Count() / pageSize);

            var specificationsDTO = new BidSpecificationsDTO
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                OnlyWinning = false
            };

            // Act
            var bidsList = await service.GetBidsListForUserAsync(user.Id, specificationsDTO);

            // Assert
            bidsList.Should().NotBeNull();
            bidsList.TotalPages.Should().Be(totalPages);
            bidsList.List.Should().NotBeNull();
            bidsList.List.Should().OnlyContain(x => x.BidderId == user.Id);
            bidsList.List.Should().BeEquivalentTo(expectedBids);
        }

        [Theory]
        [InlineData(5, 1)]
        [InlineData(3, 2)]
        [InlineData(4, 1)]
        [InlineData(2, 3)]
        [InlineData(1, 4)]
        public async Task GetBidsListForUserAsync_RequestOnlyWinning_ReturnsBidsList(int pageSize, int pageNumber)
        {
            // Arrange
            var auction1 = this.GetTestAuction();
            var auction2 = this.GetTestAuction();
            var auction3 = this.GetTestAuction();
            var auction4 = this.GetTestAuction();
            var auction5 = this.GetTestAuction();

            var user = this.GetTestUser();

            var allBids = new List<Bid> 
            { 
                this.GetTestBid(auction1, user, isWinning: true), 
                this.GetTestBid(auction2, user, isWinning: true), 
                this.GetTestBid(auction3, user, isWinning: true), 
                this.GetTestBid(auction4, user, isWinning: true), 
                this.GetTestBid(auction5, user, isWinning: true),
            };

            var bids = allBids
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.DateAndTime)
                .ToList();

            var expectedBids = bids
                .Select(x => new BidDTO
                {
                    Id = x.Id,
                    BidderId = x.BidderId,
                    BidderUsername = x.Bidder.Username,
                    AuctionId = x.AuctionId,
                    AuctionName = x.Auction.Name,
                    DateAndTime = x.DateAndTime,
                    Amount = x.Amount
                })
                .ToList();

            repositoryMock.Setup(x => x.AnyAsync<User>(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(true);
            repositoryMock.Setup(x => x.GetAsync<Bid>(It.IsAny<ISpecification<Bid>>(), false))
                .ReturnsAsync(bids);
            repositoryMock.Setup(x => x.CountAsync<Bid>(It.IsAny<Expression<Func<Bid, bool>>>()))
                .ReturnsAsync(expectedBids.Count());

            var specificationsDTO = new BidSpecificationsDTO
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                OnlyWinning = true
            };

            var totalPages = (int)Math.Ceiling((double)bids.Count() / pageSize);

            // Act
            var bidsList = await service.GetBidsListForUserAsync(user.Id, specificationsDTO);

            // Assert
            bidsList.Should().NotBeNull();
            bidsList.TotalPages.Should().Be(totalPages);
            bidsList.List.Should().NotBeNull();
            bidsList.List.Should().OnlyContain(x => x.BidderId == user.Id);
            bidsList.List.Should().BeEquivalentTo(expectedBids);
        }
    }
}
