using HCP.Repository.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<HomeService> HomeServices { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<ServiceCleaningMethod> ServiceCleaningMethods { get; set; }
        public DbSet<ServiceOption> ServiceOptions { get; set; }
        public DbSet<ServiceOptionValue> ServiceOptionValues { get; set; }
        public DbSet<ServiceStep> ServiceSteps { get; set; }
        public DbSet<ServiceAddon> ServiceAddons { get; set; }
        public DbSet<CleaningMethod> CleaningMethods { get; set; }
        public DbSet<ServiceStep> CleaningSteps { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
