/****** Object:  Database [hcp-db]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE DATABASE [hcp-db]  (EDITION = 'Basic', SERVICE_OBJECTIVE = 'Basic', MAXSIZE = 2 GB) WITH CATALOG_COLLATION = SQL_Latin1_General_CP1_CI_AS, LEDGER = OFF;
GO
ALTER DATABASE [hcp-db] SET COMPATIBILITY_LEVEL = 160
GO
ALTER DATABASE [hcp-db] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [hcp-db] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [hcp-db] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [hcp-db] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [hcp-db] SET ARITHABORT OFF 
GO
ALTER DATABASE [hcp-db] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [hcp-db] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [hcp-db] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [hcp-db] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [hcp-db] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [hcp-db] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [hcp-db] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [hcp-db] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [hcp-db] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [hcp-db] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [hcp-db] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [hcp-db] SET  MULTI_USER 
GO
ALTER DATABASE [hcp-db] SET ENCRYPTION ON
GO
ALTER DATABASE [hcp-db] SET QUERY_STORE = ON
GO
ALTER DATABASE [hcp-db] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 7), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 10, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/*** The scripts of database scoped configurations in Azure should be executed inside the target database connection. ***/
GO
-- ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 8;
GO
/****** Object:  Schema [HangFire]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE SCHEMA [HangFire]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdditionalServices]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdditionalServices](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[CleaningServiceId] [uniqueidentifier] NOT NULL,
	[Amount] [float] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Url] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Duration] [float] NULL,
 CONSTRAINT [PK_AdditionalServices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Addresses]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Addresses](
	[Id] [uniqueidentifier] NOT NULL,
	[AddressLine1] [nvarchar](255) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[PlaceId] [nvarchar](max) NOT NULL,
	[District] [nvarchar](100) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[Title] [nvarchar](50) NULL,
 CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Birthday] [datetime2](7) NULL,
	[Avatar] [nvarchar](max) NOT NULL,
	[PDF] [nvarchar](max) NULL,
	[Gender] [bit] NOT NULL,
	[BalanceWallet] [float] NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[IdCardBack] [nvarchar](max) NULL,
	[IdCardFront] [nvarchar](max) NULL,
	[HousekeeperVerifiedBy] [nvarchar](max) NULL,
	[HousekeeperStatus] [nvarchar](max) NULL,
	[FcmToken] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingAdditionals]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingAdditionals](
	[Id] [uniqueidentifier] NOT NULL,
	[AdditionalServiceId] [uniqueidentifier] NOT NULL,
	[BookingId] [uniqueidentifier] NOT NULL,
	[Amount] [float] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_BookingAdditionals] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingFinishProofs]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingFinishProofs](
	[Id] [uniqueidentifier] NOT NULL,
	[BookingId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[ImgUrl] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_BookingFinishProofs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bookings]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bookings](
	[Id] [uniqueidentifier] NOT NULL,
	[PreferDateStart] [datetime2](7) NOT NULL,
	[TimeStart] [time](7) NOT NULL,
	[TimeEnd] [time](7) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[TotalPrice] [decimal](18, 2) NOT NULL,
	[CompletedAt] [datetime2](7) NULL,
	[Rating] [float] NULL,
	[Feedback] [nvarchar](max) NULL,
	[CustomerId] [nvarchar](450) NOT NULL,
	[CleaningServiceId] [uniqueidentifier] NOT NULL,
	[Note] [nvarchar](max) NOT NULL,
	[City] [nvarchar](max) NOT NULL,
	[District] [nvarchar](max) NOT NULL,
	[PlaceId] [nvarchar](max) NOT NULL,
	[AddressLine] [nvarchar](max) NOT NULL,
	[AddtionalPrice] [decimal](18, 2) NOT NULL,
	[DistancePrice] [decimal](18, 2) NOT NULL,
	[ServicePrice] [decimal](18, 2) NOT NULL,
	[isRating] [bit] NOT NULL,
	[Fee] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [uniqueidentifier] NOT NULL,
	[CategoryName] [nvarchar](max) NOT NULL,
	[PictureUrl] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Checkout]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Checkout](
	[Id] [uniqueidentifier] NOT NULL,
	[CleaningServiceId] [uniqueidentifier] NOT NULL,
	[CustomerId] [nvarchar](450) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[TotalPrice] [decimal](18, 2) NOT NULL,
	[ServicePrice] [decimal](18, 2) NOT NULL,
	[DistancePrice] [decimal](18, 2) NOT NULL,
	[AdditionalPrice] [decimal](18, 2) NOT NULL,
	[Note] [nvarchar](max) NOT NULL,
	[City] [nvarchar](max) NOT NULL,
	[District] [nvarchar](max) NOT NULL,
	[PlaceId] [nvarchar](max) NOT NULL,
	[AddressLine] [nvarchar](max) NOT NULL,
	[TimeSLotId] [uniqueidentifier] NOT NULL,
	[DayOfWeek] [nvarchar](max) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[ServiceName] [nvarchar](max) NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[AddressId] [uniqueidentifier] NOT NULL,
	[BookingDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Checkout] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CheckoutAdditionalService]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CheckoutAdditionalService](
	[Id] [uniqueidentifier] NOT NULL,
	[AdditionalServiceId] [uniqueidentifier] NOT NULL,
	[CheckoutId] [uniqueidentifier] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Duration] [float] NULL,
	[Url] [nvarchar](max) NULL,
	[AdditionalServiceName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_CheckoutAdditionalService] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CleaningServices]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CleaningServices](
	[Id] [uniqueidentifier] NOT NULL,
	[ServiceName] [nvarchar](max) NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[Rating] [decimal](18, 2) NOT NULL,
	[RatingCount] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[City] [nvarchar](max) NOT NULL,
	[District] [nvarchar](max) NOT NULL,
	[PlaceId] [nvarchar](max) NOT NULL,
	[AddressLine] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[Duration] [float] NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[StaffId] [nvarchar](200) NULL,
	[Cordinate] [nvarchar](max) NULL,
 CONSTRAINT [PK_CleaningServices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Commissions]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Commissions](
	[Id] [uniqueidentifier] NOT NULL,
	[CommisionRate] [float] NOT NULL,
 CONSTRAINT [PK_Commissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DistancePricingRules]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DistancePricingRules](
	[Id] [uniqueidentifier] NOT NULL,
	[MinDistance] [float] NOT NULL,
	[MaxDistance] [float] NOT NULL,
	[BaseFee] [decimal](18, 2) NOT NULL,
	[ExtraPerKm] [decimal](18, 2) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CleaningServiceId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_DistancePricingRules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HousekeeperPackages]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HousekeeperPackages](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[PackageId] [uniqueidentifier] NOT NULL,
	[Duration] [float] NOT NULL,
	[SubscriptionDuration] [float] NOT NULL,
	[Type] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_HousekeeperPackages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HousekeeperSkills]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HousekeeperSkills](
	[Id] [uniqueidentifier] NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[HousekeeperId] [nvarchar](450) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[SkillLevel] [int] NOT NULL,
 CONSTRAINT [PK_HousekeeperSkills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Packages]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Packages](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Duration] [float] NOT NULL,
	[Version] [nvarchar](max) NOT NULL,
	[Type] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Packages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[Id] [uniqueidentifier] NOT NULL,
	[BookingId] [uniqueidentifier] NOT NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
	[PaymentMethod] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlatformNotifications]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlatformNotifications](
	[Id] [uniqueidentifier] NOT NULL,
	[Content] [nvarchar](max) NULL,
	[IsRead] [bit] NOT NULL,
	[ReturnUrl] [nvarchar](max) NULL,
	[SenderId] [nvarchar](450) NULL,
	[ReceiverId] [nvarchar](450) NULL,
 CONSTRAINT [PK_PlatformNotifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RefundRequests]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefundRequests](
	[Id] [uniqueidentifier] NOT NULL,
	[BookingId] [uniqueidentifier] NOT NULL,
	[Reason] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[ProofOfPayment] [nvarchar](max) NOT NULL,
	[AcceptBy] [nvarchar](450) NULL,
	[ResolutionDate] [datetime2](7) NULL,
 CONSTRAINT [PK_RefundRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceImages]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceImages](
	[Id] [uniqueidentifier] NOT NULL,
	[CleaningServiceId] [uniqueidentifier] NOT NULL,
	[LinkUrl] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ServiceImages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceRatings]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceRatings](
	[Id] [uniqueidentifier] NOT NULL,
	[Rating] [decimal](18, 2) NOT NULL,
	[Review] [nvarchar](max) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[CleaningServiceId] [uniqueidentifier] NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[RatingDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ServiceRatings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceSteps]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceSteps](
	[Id] [uniqueidentifier] NOT NULL,
	[StepOrder] [int] NOT NULL,
	[StepDescription] [nvarchar](max) NOT NULL,
	[ServiceId] [uniqueidentifier] NOT NULL,
	[Duration] [float] NULL,
 CONSTRAINT [PK_ServiceSteps] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceTimeSlots]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceTimeSlots](
	[Id] [uniqueidentifier] NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[DayOfWeek] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[ServiceId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ServiceTimeSlots] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemWallets]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemWallets](
	[Id] [uniqueidentifier] NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_SystemWallets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WalletTransactions]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WalletTransactions](
	[Id] [uniqueidentifier] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Current] [decimal](18, 2) NOT NULL,
	[AfterAmount] [decimal](18, 2) NOT NULL,
	[Type] [nvarchar](max) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ReferenceId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_WalletTransactions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[AggregatedCounter]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[AggregatedCounter](
	[Key] [nvarchar](100) NOT NULL,
	[Value] [bigint] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_CounterAggregated] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Counter]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Counter](
	[Key] [nvarchar](100) NOT NULL,
	[Value] [int] NOT NULL,
	[ExpireAt] [datetime] NULL,
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_HangFire_Counter] PRIMARY KEY CLUSTERED 
(
	[Key] ASC,
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Hash]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Hash](
	[Key] [nvarchar](100) NOT NULL,
	[Field] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ExpireAt] [datetime2](7) NULL,
 CONSTRAINT [PK_HangFire_Hash] PRIMARY KEY CLUSTERED 
(
	[Key] ASC,
	[Field] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Job]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Job](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[StateId] [bigint] NULL,
	[StateName] [nvarchar](20) NULL,
	[InvocationData] [nvarchar](max) NOT NULL,
	[Arguments] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Job] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[JobParameter]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[JobParameter](
	[JobId] [bigint] NOT NULL,
	[Name] [nvarchar](40) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_HangFire_JobParameter] PRIMARY KEY CLUSTERED 
(
	[JobId] ASC,
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[JobQueue]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[JobQueue](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobId] [bigint] NOT NULL,
	[Queue] [nvarchar](50) NOT NULL,
	[FetchedAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_JobQueue] PRIMARY KEY CLUSTERED 
(
	[Queue] ASC,
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[List]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[List](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_List] PRIMARY KEY CLUSTERED 
(
	[Key] ASC,
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Schema]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Schema](
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_HangFire_Schema] PRIMARY KEY CLUSTERED 
(
	[Version] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Server]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Server](
	[Id] [nvarchar](200) NOT NULL,
	[Data] [nvarchar](max) NULL,
	[LastHeartbeat] [datetime] NOT NULL,
 CONSTRAINT [PK_HangFire_Server] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[Set]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[Set](
	[Key] [nvarchar](100) NOT NULL,
	[Score] [float] NOT NULL,
	[Value] [nvarchar](256) NOT NULL,
	[ExpireAt] [datetime] NULL,
 CONSTRAINT [PK_HangFire_Set] PRIMARY KEY CLUSTERED 
(
	[Key] ASC,
	[Value] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [HangFire].[State]    Script Date: 5/28/2025 7:47:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [HangFire].[State](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobId] [bigint] NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Reason] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Data] [nvarchar](max) NULL,
 CONSTRAINT [PK_HangFire_State] PRIMARY KEY CLUSTERED 
(
	[JobId] ASC,
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_AdditionalServices_CleaningServiceId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AdditionalServices_CleaningServiceId] ON [dbo].[AdditionalServices]
(
	[CleaningServiceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Addresses_UserId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_Addresses_UserId] ON [dbo].[Addresses]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BookingAdditionals_AdditionalServiceId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_BookingAdditionals_AdditionalServiceId] ON [dbo].[BookingAdditionals]
(
	[AdditionalServiceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BookingAdditionals_BookingId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_BookingAdditionals_BookingId] ON [dbo].[BookingAdditionals]
(
	[BookingId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BookingFinishProofs_BookingId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_BookingFinishProofs_BookingId] ON [dbo].[BookingFinishProofs]
(
	[BookingId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Bookings_CleaningServiceId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_CleaningServiceId] ON [dbo].[Bookings]
(
	[CleaningServiceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Bookings_CustomerId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_Bookings_CustomerId] ON [dbo].[Bookings]
(
	[CustomerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Checkout_CustomerId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_Checkout_CustomerId] ON [dbo].[Checkout]
(
	[CustomerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CheckoutAdditionalService_AdditionalServiceId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_CheckoutAdditionalService_AdditionalServiceId] ON [dbo].[CheckoutAdditionalService]
(
	[AdditionalServiceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CheckoutAdditionalService_CheckoutId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_CheckoutAdditionalService_CheckoutId] ON [dbo].[CheckoutAdditionalService]
(
	[CheckoutId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CleaningServices_CategoryId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_CleaningServices_CategoryId] ON [dbo].[CleaningServices]
(
	[CategoryId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CleaningServices_UserId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_CleaningServices_UserId] ON [dbo].[CleaningServices]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_DistancePricingRules_CleaningServiceId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_DistancePricingRules_CleaningServiceId] ON [dbo].[DistancePricingRules]
(
	[CleaningServiceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HousekeeperPackages_PackageId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HousekeeperPackages_PackageId] ON [dbo].[HousekeeperPackages]
(
	[PackageId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HousekeeperPackages_UserId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HousekeeperPackages_UserId] ON [dbo].[HousekeeperPackages]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HousekeeperSkills_CategoryId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HousekeeperSkills_CategoryId] ON [dbo].[HousekeeperSkills]
(
	[CategoryId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HousekeeperSkills_HousekeeperId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HousekeeperSkills_HousekeeperId] ON [dbo].[HousekeeperSkills]
(
	[HousekeeperId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_BookingId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_BookingId] ON [dbo].[Payments]
(
	[BookingId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PlatformNotifications_ReceiverId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_PlatformNotifications_ReceiverId] ON [dbo].[PlatformNotifications]
(
	[ReceiverId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PlatformNotifications_SenderId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_PlatformNotifications_SenderId] ON [dbo].[PlatformNotifications]
(
	[SenderId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RefundRequests_AcceptBy]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_RefundRequests_AcceptBy] ON [dbo].[RefundRequests]
(
	[AcceptBy] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RefundRequests_BookingId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_RefundRequests_BookingId] ON [dbo].[RefundRequests]
(
	[BookingId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ServiceImages_CleaningServiceId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_ServiceImages_CleaningServiceId] ON [dbo].[ServiceImages]
(
	[CleaningServiceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ServiceRatings_CleaningServiceId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_ServiceRatings_CleaningServiceId] ON [dbo].[ServiceRatings]
(
	[CleaningServiceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ServiceRatings_UserId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_ServiceRatings_UserId] ON [dbo].[ServiceRatings]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ServiceSteps_ServiceId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_ServiceSteps_ServiceId] ON [dbo].[ServiceSteps]
(
	[ServiceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ServiceTimeSlots_ServiceId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_ServiceTimeSlots_ServiceId] ON [dbo].[ServiceTimeSlots]
(
	[ServiceId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_WalletTransactions_UserId]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_WalletTransactions_UserId] ON [dbo].[WalletTransactions]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_AggregatedCounter_ExpireAt]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_AggregatedCounter_ExpireAt] ON [HangFire].[AggregatedCounter]
(
	[ExpireAt] ASC
)
WHERE ([ExpireAt] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_Hash_ExpireAt]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Hash_ExpireAt] ON [HangFire].[Hash]
(
	[ExpireAt] ASC
)
WHERE ([ExpireAt] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_Job_ExpireAt]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Job_ExpireAt] ON [HangFire].[Job]
(
	[ExpireAt] ASC
)
INCLUDE([StateName]) 
WHERE ([ExpireAt] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HangFire_Job_StateName]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Job_StateName] ON [HangFire].[Job]
(
	[StateName] ASC
)
WHERE ([StateName] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_List_ExpireAt]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_List_ExpireAt] ON [HangFire].[List]
(
	[ExpireAt] ASC
)
WHERE ([ExpireAt] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_Server_LastHeartbeat]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Server_LastHeartbeat] ON [HangFire].[Server]
(
	[LastHeartbeat] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_Set_ExpireAt]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Set_ExpireAt] ON [HangFire].[Set]
(
	[ExpireAt] ASC
)
WHERE ([ExpireAt] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_HangFire_Set_Score]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_Set_Score] ON [HangFire].[Set]
(
	[Key] ASC,
	[Score] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HangFire_State_CreatedAt]    Script Date: 5/28/2025 7:47:46 AM ******/
CREATE NONCLUSTERED INDEX [IX_HangFire_State_CreatedAt] ON [HangFire].[State]
(
	[CreatedAt] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT ((0.0)) FOR [AddtionalPrice]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT ((0.0)) FOR [DistancePrice]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT ((0.0)) FOR [ServicePrice]
GO
ALTER TABLE [dbo].[Bookings] ADD  DEFAULT (CONVERT([bit],(0))) FOR [isRating]
GO
ALTER TABLE [dbo].[Checkout] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [TimeSLotId]
GO
ALTER TABLE [dbo].[Checkout] ADD  DEFAULT (N'') FOR [DayOfWeek]
GO
ALTER TABLE [dbo].[Checkout] ADD  DEFAULT ('00:00:00') FOR [EndTime]
GO
ALTER TABLE [dbo].[Checkout] ADD  DEFAULT (N'') FOR [ServiceName]
GO
ALTER TABLE [dbo].[Checkout] ADD  DEFAULT ('00:00:00') FOR [StartTime]
GO
ALTER TABLE [dbo].[Checkout] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [AddressId]
GO
ALTER TABLE [dbo].[Checkout] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [BookingDate]
GO
ALTER TABLE [dbo].[CheckoutAdditionalService] ADD  DEFAULT (N'') FOR [AdditionalServiceName]
GO
ALTER TABLE [dbo].[ServiceRatings] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [RatingDate]
GO
ALTER TABLE [dbo].[WalletTransactions] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [CreatedDate]
GO
ALTER TABLE [dbo].[WalletTransactions] ADD  DEFAULT (N'') FOR [Status]
GO
ALTER TABLE [dbo].[AdditionalServices]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalServices_CleaningServices_CleaningServiceId] FOREIGN KEY([CleaningServiceId])
REFERENCES [dbo].[CleaningServices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdditionalServices] CHECK CONSTRAINT [FK_AdditionalServices_CleaningServices_CleaningServiceId]
GO
ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD  CONSTRAINT [FK_Addresses_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Addresses] CHECK CONSTRAINT [FK_Addresses_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[BookingAdditionals]  WITH CHECK ADD  CONSTRAINT [FK_BookingAdditionals_AdditionalServices_AdditionalServiceId] FOREIGN KEY([AdditionalServiceId])
REFERENCES [dbo].[AdditionalServices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BookingAdditionals] CHECK CONSTRAINT [FK_BookingAdditionals_AdditionalServices_AdditionalServiceId]
GO
ALTER TABLE [dbo].[BookingAdditionals]  WITH CHECK ADD  CONSTRAINT [FK_BookingAdditionals_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([Id])
GO
ALTER TABLE [dbo].[BookingAdditionals] CHECK CONSTRAINT [FK_BookingAdditionals_Bookings_BookingId]
GO
ALTER TABLE [dbo].[BookingFinishProofs]  WITH CHECK ADD  CONSTRAINT [FK_BookingFinishProofs_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BookingFinishProofs] CHECK CONSTRAINT [FK_BookingFinishProofs_Bookings_BookingId]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_AspNetUsers_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_AspNetUsers_CustomerId]
GO
ALTER TABLE [dbo].[Bookings]  WITH CHECK ADD  CONSTRAINT [FK_Bookings_CleaningServices_CleaningServiceId] FOREIGN KEY([CleaningServiceId])
REFERENCES [dbo].[CleaningServices] ([Id])
GO
ALTER TABLE [dbo].[Bookings] CHECK CONSTRAINT [FK_Bookings_CleaningServices_CleaningServiceId]
GO
ALTER TABLE [dbo].[Checkout]  WITH CHECK ADD  CONSTRAINT [FK_Checkout_AspNetUsers_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Checkout] CHECK CONSTRAINT [FK_Checkout_AspNetUsers_CustomerId]
GO
ALTER TABLE [dbo].[CheckoutAdditionalService]  WITH CHECK ADD  CONSTRAINT [FK_CheckoutAdditionalService_AdditionalServices_AdditionalServiceId] FOREIGN KEY([AdditionalServiceId])
REFERENCES [dbo].[AdditionalServices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CheckoutAdditionalService] CHECK CONSTRAINT [FK_CheckoutAdditionalService_AdditionalServices_AdditionalServiceId]
GO
ALTER TABLE [dbo].[CheckoutAdditionalService]  WITH CHECK ADD  CONSTRAINT [FK_CheckoutAdditionalService_Checkout_CheckoutId] FOREIGN KEY([CheckoutId])
REFERENCES [dbo].[Checkout] ([Id])
GO
ALTER TABLE [dbo].[CheckoutAdditionalService] CHECK CONSTRAINT [FK_CheckoutAdditionalService_Checkout_CheckoutId]
GO
ALTER TABLE [dbo].[CleaningServices]  WITH CHECK ADD  CONSTRAINT [FK_CleaningServices_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CleaningServices] CHECK CONSTRAINT [FK_CleaningServices_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[CleaningServices]  WITH CHECK ADD  CONSTRAINT [FK_CleaningServices_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CleaningServices] CHECK CONSTRAINT [FK_CleaningServices_Categories_CategoryId]
GO
ALTER TABLE [dbo].[DistancePricingRules]  WITH CHECK ADD  CONSTRAINT [FK_DistancePricingRules_CleaningServices_CleaningServiceId] FOREIGN KEY([CleaningServiceId])
REFERENCES [dbo].[CleaningServices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DistancePricingRules] CHECK CONSTRAINT [FK_DistancePricingRules_CleaningServices_CleaningServiceId]
GO
ALTER TABLE [dbo].[HousekeeperPackages]  WITH CHECK ADD  CONSTRAINT [FK_HousekeeperPackages_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HousekeeperPackages] CHECK CONSTRAINT [FK_HousekeeperPackages_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[HousekeeperPackages]  WITH CHECK ADD  CONSTRAINT [FK_HousekeeperPackages_Packages_PackageId] FOREIGN KEY([PackageId])
REFERENCES [dbo].[Packages] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HousekeeperPackages] CHECK CONSTRAINT [FK_HousekeeperPackages_Packages_PackageId]
GO
ALTER TABLE [dbo].[HousekeeperSkills]  WITH CHECK ADD  CONSTRAINT [FK_HousekeeperSkills_AspNetUsers_HousekeeperId] FOREIGN KEY([HousekeeperId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HousekeeperSkills] CHECK CONSTRAINT [FK_HousekeeperSkills_AspNetUsers_HousekeeperId]
GO
ALTER TABLE [dbo].[HousekeeperSkills]  WITH CHECK ADD  CONSTRAINT [FK_HousekeeperSkills_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HousekeeperSkills] CHECK CONSTRAINT [FK_HousekeeperSkills_Categories_CategoryId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Bookings_BookingId]
GO
ALTER TABLE [dbo].[PlatformNotifications]  WITH CHECK ADD  CONSTRAINT [FK_PlatformNotifications_AspNetUsers_ReceiverId] FOREIGN KEY([ReceiverId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[PlatformNotifications] CHECK CONSTRAINT [FK_PlatformNotifications_AspNetUsers_ReceiverId]
GO
ALTER TABLE [dbo].[PlatformNotifications]  WITH CHECK ADD  CONSTRAINT [FK_PlatformNotifications_AspNetUsers_SenderId] FOREIGN KEY([SenderId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[PlatformNotifications] CHECK CONSTRAINT [FK_PlatformNotifications_AspNetUsers_SenderId]
GO
ALTER TABLE [dbo].[RefundRequests]  WITH CHECK ADD  CONSTRAINT [FK_RefundRequests_AspNetUsers_AcceptBy] FOREIGN KEY([AcceptBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[RefundRequests] CHECK CONSTRAINT [FK_RefundRequests_AspNetUsers_AcceptBy]
GO
ALTER TABLE [dbo].[RefundRequests]  WITH CHECK ADD  CONSTRAINT [FK_RefundRequests_Bookings_BookingId] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Bookings] ([Id])
GO
ALTER TABLE [dbo].[RefundRequests] CHECK CONSTRAINT [FK_RefundRequests_Bookings_BookingId]
GO
ALTER TABLE [dbo].[ServiceImages]  WITH CHECK ADD  CONSTRAINT [FK_ServiceImages_CleaningServices_CleaningServiceId] FOREIGN KEY([CleaningServiceId])
REFERENCES [dbo].[CleaningServices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ServiceImages] CHECK CONSTRAINT [FK_ServiceImages_CleaningServices_CleaningServiceId]
GO
ALTER TABLE [dbo].[ServiceRatings]  WITH CHECK ADD  CONSTRAINT [FK_ServiceRatings_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ServiceRatings] CHECK CONSTRAINT [FK_ServiceRatings_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[ServiceRatings]  WITH CHECK ADD  CONSTRAINT [FK_ServiceRatings_CleaningServices_CleaningServiceId] FOREIGN KEY([CleaningServiceId])
REFERENCES [dbo].[CleaningServices] ([Id])
GO
ALTER TABLE [dbo].[ServiceRatings] CHECK CONSTRAINT [FK_ServiceRatings_CleaningServices_CleaningServiceId]
GO
ALTER TABLE [dbo].[ServiceSteps]  WITH CHECK ADD  CONSTRAINT [FK_ServiceSteps_CleaningServices_ServiceId] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[CleaningServices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ServiceSteps] CHECK CONSTRAINT [FK_ServiceSteps_CleaningServices_ServiceId]
GO
ALTER TABLE [dbo].[ServiceTimeSlots]  WITH CHECK ADD  CONSTRAINT [FK_ServiceTimeSlots_CleaningServices_ServiceId] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[CleaningServices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ServiceTimeSlots] CHECK CONSTRAINT [FK_ServiceTimeSlots_CleaningServices_ServiceId]
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD  CONSTRAINT [FK_WalletTransactions_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WalletTransactions] CHECK CONSTRAINT [FK_WalletTransactions_AspNetUsers_UserId]
GO
ALTER TABLE [HangFire].[JobParameter]  WITH CHECK ADD  CONSTRAINT [FK_HangFire_JobParameter_Job] FOREIGN KEY([JobId])
REFERENCES [HangFire].[Job] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [HangFire].[JobParameter] CHECK CONSTRAINT [FK_HangFire_JobParameter_Job]
GO
ALTER TABLE [HangFire].[State]  WITH CHECK ADD  CONSTRAINT [FK_HangFire_State_Job] FOREIGN KEY([JobId])
REFERENCES [HangFire].[Job] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [HangFire].[State] CHECK CONSTRAINT [FK_HangFire_State_Job]
GO
ALTER DATABASE [hcp-db] SET  READ_WRITE 
GO
