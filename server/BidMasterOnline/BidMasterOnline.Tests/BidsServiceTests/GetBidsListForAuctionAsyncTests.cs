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
    /// Class for IBidsService.GetBidsListForAuctionAsync method unit tests.
    /// </summary>
    public class GetBidsListForAuctionAsyncTests : BidsServiceTestsBase
    {
        [Fact]
        public async Task GetBidsListForAuctionAsync_SpecificationsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            SpecificationsDTO specifications = null;

            // Assert
            var action = async () =>
            {
                // Act
                var bidsList = await service.GetBidsListForAuctionAsync(idToPass, specifications);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetBidsListForAuctionAsync_IncorrectAuctionId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var idToPass = Guid.NewGuid();

            repositoryMock.Setup(x => x.AnyAsync<Auction>(It.IsAny<Expression<Func<Auction, bool>>>()))
                .ReturnsAsync(false);

            var specifications = fixture.Create<SpecificationsDTO>();

            // Assert
            var action = async () =>
            {
                // Act
                var bidsList = await service.GetBidsListForAuctionAsync(idToPass, specifications);
            };

            await action.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Theory]
        [InlineData(5, 1)]
        [InlineData(3, 2)]
        [InlineData(4, 1)]
        [InlineData(2, 3)]
        [InlineData(1, 4)]
        public async Task GetBidsListForAuctionAsync_ValidId_ReturnsBidsList(int pageSize, int pageNumber)
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

            repositoryMock.Setup(x => x.AnyAsync<Auction>(It.IsAny<Expression<Func<Auction, bool>>>()))
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

            var totalPages = (int) Math.Ceiling((double) allBids.Count() / pageSize);

            var specificationsDTO = new SpecificationsDTO
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            // Act
            var bidsList = await service.GetBidsListForAuctionAsync(auction.Id, specificationsDTO);

            // Assert
            bidsList.Should().NotBeNull();
            bidsList.TotalPages.Should().Be(totalPages);
            bidsList.List.Should().NotBeNull();
            bidsList.List.Should().OnlyContain(x => x.AuctionId == auction.Id);
            bidsList.List.Should().BeEquivalentTo(expectedBids);
        }
    }
}
