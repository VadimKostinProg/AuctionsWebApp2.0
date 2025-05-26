using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BidMasterOnline.Infrastructure
{
    public class ApplicationContext : DbContext
    {
        private readonly IUserAccessor _userAccessor;

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
        public virtual DbSet<SupportTicket> TechnicalSupportRequests { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WatchList> WatchLists { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Delivery> Deliveries { get; set; }
        public virtual DbSet<ModerationLog> ModerationLogs { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options,
            IUserAccessor userAccessor) : base(options)
        {
            _userAccessor = userAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(options =>
            {
                options.HasMany<UserFeedback>()
                    .WithOne(uf => uf.FromUser)
                    .HasForeignKey(uf => uf.FromUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasMany<UserFeedback>()
                    .WithOne(uf => uf.ToUser)
                    .HasForeignKey(uf => uf.ToUserId);

                options.HasOne(x => x.Role)
                    .WithMany()
                    .HasForeignKey(u => u.RoleId);

                options.HasMany<WatchList>()
                    .WithOne()
                    .HasForeignKey(wl => wl.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasMany<Bid>()
                    .WithOne(b => b.Bidder)
                    .HasForeignKey(b => b.BidderId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasMany<AuctionComment>()
                    .WithOne(c => c.User)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Auction>(options =>
            {
                options.HasOne(a => a.Category)
                    .WithMany()
                    .HasForeignKey(a => a.AuctionCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(a => a.Type)
                    .WithMany()
                    .HasForeignKey(a => a.AuctionTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(a => a.FinishMethod)
                    .WithMany()
                    .HasForeignKey(a => a.AuctionFinishMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(a => a.Auctionist)
                    .WithMany()
                    .HasForeignKey(a => a.AuctionistId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(a => a.Winner)
                    .WithMany()
                    .HasForeignKey(a => a.WinnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasMany(a => a.Bids)
                    .WithOne(b => b.Auction)
                    .HasForeignKey(b => b.AuctionId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasMany(a => a.Images)
                    .WithOne()
                    .HasForeignKey(i => i.AuctionId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasMany<WatchList>()
                    .WithOne(wl => wl.Auction)
                    .HasForeignKey(wl => wl.AuctionId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasMany<AuctionComment>()
                    .WithOne(c => c.Auction)
                    .HasForeignKey(c => c.AuctionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AuctionRequest>(options =>
            {
                options.HasOne(ar => ar.Category)
                    .WithMany()
                    .HasForeignKey(ar => ar.AuctionCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(ar => ar.Type)
                    .WithMany()
                    .HasForeignKey(ar => ar.AuctionTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(ar => ar.FinishMethod)
                    .WithMany()
                    .HasForeignKey(ar => ar.AuctionFinishMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(ar => ar.RequestedByUser)
                    .WithMany()
                    .HasForeignKey(ar => ar.RequestedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasMany(ar => ar.Images)
                    .WithOne()
                    .HasForeignKey(i => i.AuctionRequestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Complaint>(options =>
            {
                options.HasOne(c => c.AccusingUser)
                    .WithMany()
                    .HasForeignKey(c => c.AccusingUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(c => c.AccusedUser)
                    .WithMany()
                    .HasForeignKey(c => c.AccusedUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(c => c.AccusedAuction)
                    .WithMany()
                    .HasForeignKey(c => c.AccusedAuctionId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(c => c.AccusedComment)
                    .WithMany()
                    .HasForeignKey(c => c.AccusedCommentId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(c => c.AccusedUserFeedback)
                    .WithMany()
                    .HasForeignKey(c => c.AccusedUserFeedbackId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(c => c.Moderator)
                    .WithMany()
                    .HasForeignKey(c => c.ModeratorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SupportTicket>(options =>
            {
                options.HasOne(tsr => tsr.User)
                    .WithMany()
                    .HasForeignKey(tsr => tsr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne(tsr => tsr.Moderator)
                    .WithMany()
                    .HasForeignKey(tsr => tsr.ModeratorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Delivery>(options =>
            {
                options.HasOne<Auction>()
                    .WithOne()
                    .HasForeignKey<Delivery>(p => p.AuctionId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne<User>()
                    .WithOne()
                    .HasForeignKey<Delivery>(p => p.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne<User>()
                    .WithOne()
                    .HasForeignKey<Delivery>(p => p.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ModerationLog>(options =>
            {
                options.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(ml => ml.ModeratorId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(ml => ml.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne<Auction>()
                    .WithMany()
                    .HasForeignKey(ml => ml.AuctionId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne<AuctionRequest>()
                    .WithMany()
                    .HasForeignKey(ml => ml.AuctionRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                options.HasOne<AuctionComment>()
                    .WithMany()
                    .HasForeignKey(ml => ml.AuctionCommentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed Data
            // TODO: move names to consts or enums, add descriptions where required
            modelBuilder.Entity<Role>().HasData([
                    new Role
                    {
                        Id = 1,
                        Name = UserRoles.Admin
                    },
                    new Role
                    {
                        Id = 2,
                        Name = UserRoles.Moderator
                    },
                    new Role
                    {
                        Id = 3,
                        Name = UserRoles.Participant
                    }
                ]);

            modelBuilder.Entity<AuctionFinishMethod>().HasData(new List<AuctionFinishMethod>
            {
                new AuctionFinishMethod
                {
                    Id = 1,
                    Name = "Static finish method",
                    Description = "Auction finishes in the defined statis time.",
                    CreatedBy = "system"
                },
                new AuctionFinishMethod
                {
                    Id = 2,
                    Name = "Dynamic finish method",
                    Description = "Auction finish time increases on the defined interval after every new bid.",
                    CreatedBy = "system"
                }
            });

            modelBuilder.Entity<AuctionType>().HasData(new List<AuctionType>
            {
                new AuctionType
                {
                    Id = 1,
                    Name = "English Auction",
                    Description = "",
                    CreatedBy = "system"
                },
                new AuctionType
                {
                    Id = 2,
                    Name = "Dutch Auction",
                    Description = "",
                    CreatedBy = "system"
                }
            });
        }

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            IEnumerable<EntityEntry<EntityBase>> entries = ChangeTracker.Entries<EntityBase>();

            DateTime currentTime = DateTime.UtcNow;
            string currentUsername = _userAccessor.TryGetUserName() ?? "system";

            foreach (EntityBase addedEntity in entries.Where(e => e.State == EntityState.Added)
                                                      .Select(e => e.Entity))
            {
                addedEntity.CreatedAt = currentTime;
                addedEntity.CreatedBy = currentUsername;
            }

            foreach (EntityBase modifiedEntity in entries.Where(e => e.State == EntityState.Modified)
                                                         .Select(e => e.Entity))
            {
                modifiedEntity.ModifiedAt = currentTime;
                modifiedEntity.ModifiedBy = currentUsername;
            }
        }
    }
}
