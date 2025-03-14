using HCP.Repository.Constance;
using HCP.Repository.Data;
using HCP.Repository.Entities;
using HomeCleaningService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace HomeCleaningService.Extensions
{
    public static class DbContextExtension
    {
        public static IServiceCollection AddDatabaseConfig(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            return services;
        }

        public static async Task<WebApplication> AddAutoMigrateAndSeedDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            try
            {
                var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                await context.Database.MigrateAsync();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

                await SeedRolesAndUsersAsync(roleManager, userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during migration and seeding");
            }

            return app;
        }



        private static async Task SeedRolesAndUsersAsync(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            // Define roles
            string[] roles = { "Housekeeper", "Customer", KeyConst.Admin, "Staff" };

            // Create roles if they don't exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Users to seed (2 Housekeepers, 2 Customers, 2 Staff, 1 Admin)
            var users = new List<(string FirstName, string Email, string UserName, string Role)>
        {
            ("Alice", "housekeeper1@example.com", "HousekeeperAlice", "Housekeeper"),
            ("Emma", "housekeeper2@example.com", "HousekeeperEmma", "Housekeeper"),

            ("Bob", "customer1@example.com", "CustomerBob", "Customer"),
            ("David", "customer2@example.com", "CustomerDavid", "Customer"),

            ("Charlie", "staff1@example.com", "StaffCharlie", "Staff"),
            ("Eve", "staff2@example.com", "StaffEve", "Staff"),

            (KeyConst.Admin, "admin@example.com", "AdminUser", KeyConst.Admin)
        };

            foreach (var (firstName, email, userName, role) in users)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new AppUser
                    {
                        FullName = firstName,
                        Email = email,
                        NormalizedEmail = email.ToUpper(),
                        UserName = userName,
                        NormalizedUserName = userName.ToUpper(),
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };

                    var password = "123456"; // Ensure this meets Identity password policy
                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                    else
                    {
                        throw new Exception($"Failed to create user {userName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }
    }
}
