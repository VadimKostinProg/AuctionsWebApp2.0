using BidMasterOnline.Application.Constants;
using BidMasterOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BidMasterOnline.Infrastructure.DatabaseContext
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<AuctionFinishType> AuctionFinishTypes { get; set; }
        public virtual DbSet<AuctionStatus> AuctionStatuses { get; set; }
        public virtual DbSet<AuctionScore> AuctionScores { get; set; }
        public virtual DbSet<AuctionImage> AuctionImages { get; set; }
        public virtual DbSet<AuctionPaymentDeliveryOptions> AuctionPaymentDeliveryOptions { get; set; }
        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<AuctionComment> Comments { get; set; }
        public virtual DbSet<Complaint> Complaints { get; set; }
        public virtual DbSet<ComplaintType> ComplaintTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<TechnicalSupportRequest> TechnicalSupportRequests { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserStatus> UserStatuses { get; set; }

        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Auction>().ToTable("Auctions");
            modelBuilder.Entity<AuctionFinishType>().ToTable("AuctionFinishTypes");
            modelBuilder.Entity<AuctionStatus>().ToTable("AuctionStatuses");
            modelBuilder.Entity<AuctionScore>().ToTable("AuctionScores");
            modelBuilder.Entity<AuctionImage>().ToTable("AuctionImages");
            modelBuilder.Entity<AuctionPaymentDeliveryOptions>().ToTable("AuctionPaymentDeliveryOptions");
            modelBuilder.Entity<Bid>().ToTable("Bids");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<AuctionComment>().ToTable("Comments");
            modelBuilder.Entity<Complaint>().ToTable("Complaints");
            modelBuilder.Entity<ComplaintType>().ToTable("ComplaintTypes");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<TechnicalSupportRequest>().ToTable("TechnicalSupportRequests");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<UserStatus>().ToTable("UserStatuses");

            // Seed Data 
            modelBuilder.Entity<Role>().HasData(new List<Role>
            {
                new Role { Name = UserRoles.Customer },
                new Role { Name = UserRoles.Admin },
                new Role { Name = UserRoles.TechnicalSupportSpecialist },
            });

            modelBuilder.Entity<UserStatus>().HasData(new List<UserStatus>
            {
                new UserStatus { Name = Application.Enums.UserStatus.Active.ToString() },
                new UserStatus { Name = Application.Enums.UserStatus.Blocked.ToString() },
                new UserStatus { Name = Application.Enums.UserStatus.Deleted.ToString() }
            });

            modelBuilder.Entity<AuctionStatus>().HasData(new List<AuctionStatus>
            {
                new AuctionStatus { Name = Application.Enums.AuctionStatus.Active.ToString() },
                new AuctionStatus { Name = Application.Enums.AuctionStatus.Canceled.ToString() },
                new AuctionStatus { Name = Application.Enums.AuctionStatus.Finished.ToString() }
            });

            modelBuilder.Entity<AuctionFinishType>().HasData(new List<AuctionFinishType>
            {
                new AuctionFinishType
                {
                    Name = Application.Enums.AuctionFinishType.StaticFinishTime.ToString(),
                    Description = "Auction finishes in the defined statis time."
                },
                new AuctionFinishType
                {
                    Name = Application.Enums.AuctionFinishType.IncreasingFinishTime.ToString(),
                    Description = "Auction finish time increases on the defined interval after every new bid."
                }
            });

            modelBuilder.Entity<ComplaintType>().HasData(new List<ComplaintType>
            {
                new ComplaintType
                {
                    Name = Application.Enums.ComplaintType.ComplaintOnUserNonPayemnt.ToString(),
                    Description = "Complaint on user which has not payed for the lot of the auction."
                },
                new ComplaintType
                {
                    Name = Application.Enums.ComplaintType.ComplaintOnUserNonProvidingLot.ToString(),
                    Description = "Complaint on user which has not provided the lot of the auction."
                },
                new ComplaintType
                {
                    Name = Application.Enums.ComplaintType.ComplaintOnAuctionContent.ToString(),
                    Description = "Complaint on the auction content."
                },
                new ComplaintType
                {
                    Name = Application.Enums.ComplaintType.ComplaintOnUserComment.ToString(),
                    Description = "Complaint on the user comment."
                }
            });
        }
    }
}
