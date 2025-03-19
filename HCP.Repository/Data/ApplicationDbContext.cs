using HCP.Repository.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HCP.Repository.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<HousekeeperSkill> HousekeeperSkills { get; set; }
        public DbSet<HousekeeperPackage> HousekeeperPackages { get; set; }

        // Services & Categories
        public DbSet<CleaningService> CleaningServices { get; set; }
        public DbSet<ServiceCategory> Categories { get; set; }
        public DbSet<AdditionalService> AdditionalServices { get; set; }
        public DbSet<ServiceImg> ServiceImages { get; set; }
        public DbSet<ServiceSteps> ServiceSteps { get; set; }
        public DbSet<ServiceTimeSlot> ServiceTimeSlots { get; set; }

        //checkout
        public DbSet<Checkout> Checkout { get; set; }
        public DbSet<CheckoutAdditionalService> CheckoutAdditionalService { get; set; }

        // Booking & Payments
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingAdditional> BookingAdditionals { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RefundRequest> RefundRequests { get; set; }

        // Pricing & Transactions
        public DbSet<Package> Packages { get; set; }
        public DbSet<DistancePricingRule> DistancePricingRules { get; set; }
        public DbSet<Commissions> Commissions { get; set; }
        public DbSet<SystemWallet> SystemWallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }

        // Ratings & Notifications
        public DbSet<ServiceRating> ServiceRatings { get; set; }
        public DbSet<BookingFinishProof> BookingFinishProofs { get; set; }
        public DbSet<PlatformNotification> PlatformNotifications { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<CheckoutAdditionalService>()
        .HasOne(cas => cas.Checkout)
        .WithMany(c => c.CheckoutAdditionalServices)
        .HasForeignKey(cas => cas.CheckoutId)
        .OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete issue

            builder.Entity<Checkout>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
            builder.Entity<Package>()
    .Property(p => p.Price)
    .HasColumnType("decimal(18,2)");

            builder.Entity<CleaningService>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<WalletTransaction>()
                .Property(wt => wt.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<DistancePricingRule>()
                .Property(d => d.BaseFee)
                .HasColumnType("decimal(18,2)");

            builder.Entity<DistancePricingRule>()
                .Property(d => d.ExtraPerKm)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Booking>()
    .HasOne(b => b.CleaningService)
    .WithMany(cs => cs.Bookings)
    .HasForeignKey(b => b.CleaningServiceId)
    .OnDelete(DeleteBehavior.Restrict); // 🔹 Prevents cascading delete

            builder.Entity<ServiceRating>()
    .HasOne(sr => sr.CleaningService)
    .WithMany(cs => cs.ServiceRatings)
    .HasForeignKey(sr => sr.CleaningServiceId)
    .OnDelete(DeleteBehavior.Restrict); // Prevents cascading delete

            builder.Entity<BookingAdditional>()
    .HasOne(ba => ba.Booking)
    .WithMany(b => b.BookingAdditionals)
    .HasForeignKey(ba => ba.BookingId)
    .OnDelete(DeleteBehavior.Restrict); // Prevents cascading delete


            builder.Entity<RefundRequest>()
    .HasOne(rr => rr.Booking)
    .WithMany()
    .HasForeignKey(rr => rr.BookingId)
    .OnDelete(DeleteBehavior.Restrict); // Prevents cascading delete

            builder.Entity<RefundRequest>()
    .HasOne(rr => rr.Booking)
    .WithMany()
    .HasForeignKey(rr => rr.BookingId)
    .OnDelete(DeleteBehavior.Restrict); // Prevents cascading delete
        }
    }
}
