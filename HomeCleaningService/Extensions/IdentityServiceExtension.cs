using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using HCP.Repository.Entities;
using HCP.Repository.Data;

namespace HomeCleaningService.Extensions;
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityService(
           this IServiceCollection services,
           IConfiguration config
       )
        {
        // Đăng ký các dịch vụ của Identity
        services
            .AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false; 
                options.Password.RequireLowercase = false; 
                options.Password.RequireNonAlphanumeric = false; 
                options.Password.RequireUppercase = false; 
                options.Password.RequiredLength = 6; 
                options.Password.RequiredUniqueChars = 1; 

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); 
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters = 
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true; 

                options.SignIn.RequireConfirmedEmail = true; 
                options.SignIn.RequireConfirmedPhoneNumber = false; 
            });

            //// Cấu hình Cookie
            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = $"/login/";
            //    options.LogoutPath = $"/logout/";
            //    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            //});
            //services.Configure<SecurityStampValidatorOptions>(options =>
            //{
            //    // Trên 5 giây truy cập lại sẽ nạp lại thông tin User (Role)
            //    // SecurityStamp trong bảng User đổi -> nạp lại thông tinn Security
            //    options.ValidationInterval = TimeSpan.FromSeconds(5);
            //});

            //services
            //    .AddAuthentication()
            //    .AddGoogle(googleOptions =>
            //    {
            //        // Đọc thông tin Authentication:Google từ appsettings.json
            //        IConfigurationSection googleAuthNSection = config.GetSection(
            //            "Authentication:Google"
            //        );
            //        // Thiết lập ClientID và ClientSecret để truy cập API google
            //        googleOptions.ClientId = googleAuthNSection["ClientId"]!;
            //        googleOptions.ClientSecret = googleAuthNSection["ClientSecret"]!;
            //        // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
            //        googleOptions.CallbackPath = "/dang-nhap-tu-google";
            //        googleOptions.ClaimActions.MapJsonKey("image", "picture");
            //    });

            return services;
        }
    }

