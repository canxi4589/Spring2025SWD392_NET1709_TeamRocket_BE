using Hangfire;
using HCP.Repository.GenericRepository;
using HCP.Repository.Interfaces;
using HCP.Service.Integrations.BlobStorage;
using HCP.Service.Integrations.Vnpay;
using HCP.Service.Services;
using HCP.Service.Services.BookingService;
using HCP.Service.Services.CheckoutService;
using HCP.Service.Services.CleaningService1;
using HCP.Service.Services.CustomerService;
using HCP.Service.Services.EmailService;
using HCP.Service.Services.RequestService;
using HCP.Service.Services.WalletService;
using HomeCleaningService.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDatabaseConfig(config);
builder.Services.AddIdentityService(config);
builder.Services.AddBlobService(config);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestIdentity API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(30);
});
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var google = builder.Configuration.GetSection("GoogleAuth");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });
   
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder => builder
            .AllowAnyOrigin() // Specify the frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod());

});
builder.Services.AddHttpClient();
builder.Services.AddHangfire(config => config
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ITokenHelper, TokenHelper>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICleaningService1,CleaningService1>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IHandleRequestService, HandleRequestService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IBlobStorageService,BlobStorageService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<Ivnpay,VnPay>();
builder.Services.AddScoped<IBookingTransactionService, BookingTransactionService>();
builder.Services.AddScoped<IGoongDistanceService, GoongDistanceService>();
//builder.Services.AddScoped<ICleaningService, CleaningService>();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
await app.AddAutoMigrateAndSeedDatabase();

app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
