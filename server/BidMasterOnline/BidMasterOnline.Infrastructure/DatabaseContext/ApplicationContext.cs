using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BidMasterOnline.Infrastructure.DatabaseContext
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<AuctionRequest> AuctionRequests { get; set; }
        public virtual DbSet<AuctionCategory> Categories { get; set; }
        public virtual DbSet<AuctionType> AuctionTypes { get; set; }
        public virtual DbSet<AuctionFinishMethod> AuctionFinishMethods { get; set; }
        public virtual DbSet<AuctionImage> AuctionImages { get; set; }
        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<AuctionComment> Comments { get; set; }
        public virtual DbSet<Complaint> Complaints { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<TechnicalSupportRequest> TechnicalSupportRequests { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WatchList> WatchLists { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Delivery> Deliveries { get; set; }
        public virtual DbSet<ModerationLog> ModerationLogs { get; set; }

        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(options =>
            {
                options.HasMany<UserFeedback>()
                    .WithOne()
                    .HasForeignKey(uf => uf.FromUserId);

                options.HasMany<UserFeedback>()
                    .WithOne()
                    .HasForeignKey(uf => uf.ToUserId);

                options.HasOne(x => x.Role)
                    .WithMany()
                    .HasForeignKey(u => u.RoleId);

                options.HasMany<WatchList>()
                    .WithOne()
                    .HasForeignKey(wl => wl.UserId);

                options.HasMany<Bid>()
                    .WithOne(b => b.Bidder)
                    .HasForeignKey(b => b.BidderId);

                options.HasMany<AuctionComment>()
                    .WithOne(c => c.User)
                    .HasForeignKey(c => c.UserId);
            });

            modelBuilder.Entity<Auction>(options =>
            {
                options.HasOne(a => a.Category)
                    .WithMany()
                    .HasForeignKey(a => a.AuctionCategoryId);

                options.HasOne(a => a.Type)
                    .WithMany()
                    .HasForeignKey(a => a.AuctionTypeId);

                options.HasOne(a => a.FinishMethod)
                    .WithMany()
                    .HasForeignKey(a => a.AuctionFinishMethodId);

                options.HasOne(a => a.Auctionist)
                    .WithMany()
                    .HasForeignKey(a => a.AuctionistId);

                options.HasOne(a => a.Winner)
                    .WithMany()
                    .HasForeignKey(a => a.WinnerId);

                options.HasMany(a => a.Bids)
                    .WithOne()
                    .HasForeignKey(b => b.AuctionId);

                options.HasMany(a => a.Images)
                    .WithOne()
                    .HasForeignKey(i => i.AuctionId);

                options.HasMany<WatchList>()
                    .WithOne(wl => wl.Auction)
                    .HasForeignKey(wl => wl.AuctionId);

                options.HasMany<Bid>()
                    .WithOne(b => b.Auction)
                    .HasForeignKey(b => b.AuctionId);

                options.HasMany<AuctionComment>()
                    .WithOne(c => c.Auction)
                    .HasForeignKey(c => c.AuctionId);
            });

            modelBuilder.Entity<AuctionRequest>(options =>
            {
                options.HasOne(ar => ar.Category)
                    .WithMany()
                    .HasForeignKey(ar => ar.AuctionCategoryId);

                options.HasOne(ar => ar.Type)
                    .WithMany()
                    .HasForeignKey(ar => ar.AuctionTypeId);

                options.HasOne(ar => ar.FinishMethod)
                    .WithMany()
                    .HasForeignKey(ar => ar.AuctionFinishMethodId);

                options.HasOne(ar => ar.RequestedByUser)
                    .WithMany()
                    .HasForeignKey(ar => ar.RequestedByUserId);

                options.HasMany(ar => ar.Images)
                    .WithOne()
                    .HasForeignKey(i => i.AuctionRequestId);
            });

            modelBuilder.Entity<Complaint>(options =>
            {
                options.HasOne(c => c.AccusingUser)
                    .WithMany()
                    .HasForeignKey(c => c.AccusingUserId);

                options.HasOne(c => c.AccusedUser)
                    .WithMany()
                    .HasForeignKey(c => c.AccusedUserId);

                options.HasOne(c => c.AccusedAuction)
                    .WithMany()
                    .HasForeignKey(c => c.AccusedAuctionId);

                options.HasOne(c => c.AccusedComment)
                    .WithMany()
                    .HasForeignKey(c => c.AccusedCommentId);

                options.HasOne(c => c.Moderator)
                    .WithMany()
                    .HasForeignKey(c => c.ModeratorId);
            });

            modelBuilder.Entity<TechnicalSupportRequest>(options =>
            {
                options.HasOne(tsr => tsr.User)
                    .WithMany()
                    .HasForeignKey(tsr => tsr.UserId);

                options.HasOne(tsr => tsr.Moderator)
                    .WithMany()
                    .HasForeignKey(tsr => tsr.ModeratorId);
            });

            modelBuilder.Entity<Delivery>(options =>
            {
                options.HasOne<Auction>()
                    .WithOne()
                    .HasForeignKey<Delivery>(p => p.AuctionId);

                options.HasOne<User>()
                    .WithOne()
                    .HasForeignKey<Delivery>(p => p.SellerId);

                options.HasOne<User>()
                    .WithOne()
                    .HasForeignKey<Delivery>(p => p.BuyerId);
            });

            modelBuilder.Entity<ModerationLog>(options =>
            {
                options.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(ml => ml.UserId);

                options.HasOne<Auction>()
                    .WithMany()
                    .HasForeignKey(ml => ml.AuctionId);

                options.HasOne<AuctionRequest>()
                    .WithMany()
                    .HasForeignKey(ml => ml.AuctionRequestId);

                options.HasOne<AuctionComment>()
                    .WithMany()
                    .HasForeignKey(ml => ml.AuctionCommentId);

                options.HasOne<Complaint>()
                    .WithMany()
                    .HasForeignKey(ml => ml.ComplaintId);

                options.HasOne<TechnicalSupportRequest>()
                    .WithMany()
                    .HasForeignKey(ml => ml.TechnicalSupportRequestId);
            });

            // Seed Data
            // TODO: add roles, move names to consts or enums, add descriptions where required
            modelBuilder.Entity<AuctionFinishMethod>().HasData(new List<AuctionFinishMethod>
            {
                new AuctionFinishMethod
                {
                    Name = "Static finish method",
                    Description = "Auction finishes in the defined statis time.",
                    CreatedBy = "system"
                },
                new AuctionFinishMethod
                {
                    Name = "Dynamic finish method",
                    Description = "Auction finish time increases on the defined interval after every new bid.",
                    CreatedBy = "system"
                }
            });

            modelBuilder.Entity<AuctionType>().HasData(new List<AuctionType>
            {
                new AuctionType
                {
                    Name = "English",
                    Description = "",
                    CreatedBy = "system"
                },
                new AuctionType
                {
                    Name = "Golland",
                    Description = "",
                    CreatedBy = "system"
                }
            });
        }
    }
}
