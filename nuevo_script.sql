SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attraction](
	[Id] [uniqueidentifier] NOT NULL,
	[LocationId] [uniqueidentifier] NOT NULL,
	[SubcategoryId] [uniqueidentifier] NOT NULL,
	[Slug] [nvarchar](200) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[DescriptionShort] [nvarchar](255) NULL,
	[DescriptionFull] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
	[MeetingPoint] [nvarchar](max) NULL,
	[RatingAverage] [decimal](3, 2) NOT NULL,
	[RatingCount] [int] NOT NULL,
	[MinAge] [smallint] NULL,
	[MaxGroupSize] [smallint] NULL,
	[DifficultyLevel] [nvarchar](20) NULL,
	[IsActive] [bit] NOT NULL,
	[IsPublished] [bit] NOT NULL,
	[ManagedById] [uniqueidentifier] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Attraction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Attraction_Slug] UNIQUE NONCLUSTERED 
(
	[Slug] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[AttractionInclusion] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttractionInclusion](
	[AttractionId] [uniqueidentifier] NOT NULL,
	[InclusionItemId] [uniqueidentifier] NOT NULL,
	[Type] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_AttractionInclusion] PRIMARY KEY CLUSTERED 
(
	[AttractionId] ASC,
	[InclusionItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[AttractionLanguage] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttractionLanguage](
	[Id] [uniqueidentifier] NOT NULL,
	[AttractionId] [uniqueidentifier] NOT NULL,
	[LanguageId] [smallint] NOT NULL,
	[GuideType] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_AttractionLanguage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AttractionLanguage] UNIQUE NONCLUSTERED 
(
	[AttractionId] ASC,
	[LanguageId] ASC,
	[GuideType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[AttractionMedia] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttractionMedia](
	[Id] [uniqueidentifier] NOT NULL,
	[AttractionId] [uniqueidentifier] NOT NULL,
	[MediaTypeId] [smallint] NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[ThumbnailUrl] [nvarchar](max) NULL,
	[Title] [nvarchar](150) NULL,
	[LanguageId] [smallint] NULL,
	[IsMain] [bit] NOT NULL,
	[SortOrder] [smallint] NOT NULL,
	[FileSizeKb] [int] NULL,
	[DurationSecs] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_AttractionMedia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[AttractionTag] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttractionTag](
	[AttractionId] [uniqueidentifier] NOT NULL,
	[TagId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AttractionTag] PRIMARY KEY CLUSTERED 
(
	[AttractionId] ASC,
	[TagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[AttractionTranslation] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttractionTranslation](
	[Id] [uniqueidentifier] NOT NULL,
	[AttractionId] [uniqueidentifier] NOT NULL,
	[LanguageId] [smallint] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[DescriptionShort] [nvarchar](255) NULL,
	[DescriptionFull] [nvarchar](max) NULL,
	[MeetingPoint] [nvarchar](max) NULL,
 CONSTRAINT [PK_AttractionTranslation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AttractionTranslation] UNIQUE NONCLUSTERED 
(
	[AttractionId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[AudioGuide] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AudioGuide](
	[Id] [uniqueidentifier] NOT NULL,
	[AttractionId] [uniqueidentifier] NOT NULL,
	[LanguageId] [smallint] NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[TotalDurationSecs] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_AudioGuide] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AudioGuide] UNIQUE NONCLUSTERED 
(
	[AttractionId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[AudioGuideStop] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AudioGuideStop](
	[Id] [uniqueidentifier] NOT NULL,
	[AudioGuideId] [uniqueidentifier] NOT NULL,
	[StopNumber] [smallint] NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[AudioUrl] [nvarchar](max) NOT NULL,
	[DurationSecs] [int] NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
	[ImageUrl] [nvarchar](max) NULL,
 CONSTRAINT [PK_AudioGuideStop] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AudioGuideStop] UNIQUE NONCLUSTERED 
(
	[AudioGuideId] ASC,
	[StopNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[AuditLog] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLog](
	[Id] [uniqueidentifier] NOT NULL,
	[TableName] [nvarchar](100) NOT NULL,
	[RecordId] [uniqueidentifier] NOT NULL,
	[Action] [nvarchar](10) NOT NULL,
	[ChangedBy] [nvarchar](256) NULL,
	[ChangedAt] [datetime2](7) NOT NULL,
	[OldValues] [nvarchar](max) NULL,
	[NewValues] [nvarchar](max) NULL,
	[IpAddress] [nvarchar](45) NULL,
	[UserAgent] [nvarchar](500) NULL,
	[Endpoint] [nvarchar](255) NULL,
 CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[AvailabilitySlot] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AvailabilitySlot](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[SlotDate] [date] NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NULL,
	[CapacityTotal] [smallint] NOT NULL,
	[CapacityAvailable] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_AvailabilitySlot] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AvailabilitySlot] UNIQUE NONCLUSTERED 
(
	[ProductId] ASC,
	[SlotDate] ASC,
	[StartTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Booking] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booking](
	[Id] [uniqueidentifier] NOT NULL,
	[PnrCode] [nvarchar](8) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[SlotId] [uniqueidentifier] NOT NULL,
	[StatusId] [smallint] NOT NULL,
	[TotalAmount] [decimal](12, 2) NOT NULL,
	[CurrencyCode] [nvarchar](3) NOT NULL,
	[LanguageId] [smallint] NULL,
	[Notes] [nvarchar](max) NULL,
	[InternalNotes] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[CancelledAt] [datetime2](7) NULL,
	[CancelReason] [nvarchar](max) NULL,
 CONSTRAINT [PK_Booking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Booking_Pnr] UNIQUE NONCLUSTERED 
(
	[PnrCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[BookingDetail] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingDetail](
	[Id] [uniqueidentifier] NOT NULL,
	[BookingId] [uniqueidentifier] NOT NULL,
	[PriceTierId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[DocumentType] [nvarchar](20) NULL,
	[DocumentNumber] [nvarchar](50) NOT NULL,
	[Quantity] [smallint] NOT NULL,
	[UnitPrice] [decimal](12, 2) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_BookingDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[BookingStatus] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingStatus](
	[Id] [smallint] NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_BookingStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_BookingStatus_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Category] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[Id] [uniqueidentifier] NOT NULL,
	[Slug] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IconUrl] [nvarchar](max) NULL,
	[SortOrder] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Category_Slug] UNIQUE NONCLUSTERED 
(
	[Slug] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[CategoryTranslation] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CategoryTranslation](
	[Id] [uniqueidentifier] NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[LanguageId] [smallint] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_CategoryTranslation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_CategoryTranslation] UNIQUE NONCLUSTERED 
(
	[CategoryId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Client] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Client](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[BirthDate] [date] NULL,
	[Nationality] [nvarchar](100) NULL,
	[DocumentType] [nvarchar](20) NULL,
	[DocumentNumber] [nvarchar](50) NULL,
	[LocationId] [uniqueidentifier] NULL,
	[AvatarUrl] [nvarchar](max) NULL,
	[PreferredLang] [smallint] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Client_UserId] UNIQUE NONCLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[InclusionItem] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InclusionItem](
	[Id] [uniqueidentifier] NOT NULL,
	[IconSlug] [nvarchar](50) NULL,
	[DefaultText] [nvarchar](max) NOT NULL,
	[LanguageId] [smallint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_InclusionItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Language] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Language](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[IsoCode] [nvarchar](5) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Language_Iso] UNIQUE NONCLUSTERED 
(
	[IsoCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Locations] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Locations](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[ParentId] [uniqueidentifier] NULL,
	[CountryCode] [nvarchar](2) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[MediaType] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MediaType](
	[Id] [smallint] NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_MediaType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_MediaType_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Payment] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [uniqueidentifier] NOT NULL,
	[BookingId] [uniqueidentifier] NOT NULL,
	[TransactionExternalId] [nvarchar](100) NULL,
	[PaymentMethodId] [smallint] NOT NULL,
	[StatusId] [smallint] NOT NULL,
	[Amount] [decimal](12, 2) NOT NULL,
	[CurrencyCode] [nvarchar](3) NOT NULL,
	[GatewayResponse] [nvarchar](max) NULL,
	[PaidAt] [datetime2](7) NULL,
	[RefundedAt] [datetime2](7) NULL,
	[RefundReason] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[PaymentMethodType] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentMethodType](
	[Id] [smallint] NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_PaymentMethodType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_PaymentMethodType_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[PaymentStatusType] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentStatusType](
	[Id] [smallint] NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_PaymentStatusType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_PaymentStatusType_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[PriceTier] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceTier](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[TicketCategoryId] [uniqueidentifier] NOT NULL,
	[Price] [decimal](12, 2) NOT NULL,
	[CurrencyCode] [nvarchar](3) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_PriceTier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ProductInclusion] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductInclusion](
	[ProductId] [uniqueidentifier] NOT NULL,
	[InclusionItemId] [uniqueidentifier] NOT NULL,
	[Type] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_ProductInclusion] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC,
	[InclusionItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ProductOption] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductOption](
	[Id] [uniqueidentifier] NOT NULL,
	[AttractionId] [uniqueidentifier] NOT NULL,
	[Slug] [nvarchar](150) NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DurationMinutes] [int] NULL,
	[DurationDescription] [nvarchar](100) NULL,
	[CancelPolicyHours] [int] NOT NULL,
	[CancelPolicyText] [nvarchar](max) NULL,
	[MaxGroupSize] [smallint] NULL,
	[MinParticipants] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsPrivate] [bit] NOT NULL,
	[SortOrder] [smallint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ProductOption] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ProductOption_Slug] UNIQUE NONCLUSTERED 
(
	[AttractionId] ASC,
	[Slug] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ProductScheduleTemplate] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductScheduleTemplate](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Monday] [bit] NOT NULL,
	[Tuesday] [bit] NOT NULL,
	[Wednesday] [bit] NOT NULL,
	[Thursday] [bit] NOT NULL,
	[Friday] [bit] NOT NULL,
	[Saturday] [bit] NOT NULL,
	[Sunday] [bit] NOT NULL,
	[ValidFrom] [date] NOT NULL,
	[ValidTo] [date] NULL,
	[DefaultCapacity] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ProductScheduleTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ProductScheduleTime] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductScheduleTime](
	[Id] [uniqueidentifier] NOT NULL,
	[TemplateId] [uniqueidentifier] NOT NULL,
	[StartTime] [time](0) NOT NULL,
	[EndTime] [time](0) NULL,
	[CapacityOverride] [smallint] NULL,
	[SortOrder] [smallint] NOT NULL,
 CONSTRAINT [PK_ProductScheduleTime] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ProductTranslation] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductTranslation](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[LanguageId] [smallint] NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DurationDescription] [nvarchar](100) NULL,
	[CancelPolicyText] [nvarchar](max) NULL,
 CONSTRAINT [PK_ProductTranslation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ProductTranslation] UNIQUE NONCLUSTERED 
(
	[ProductId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Review] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Review](
	[Id] [uniqueidentifier] NOT NULL,
	[BookingId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[OverallScore] [decimal](3, 2) NOT NULL,
	[Title] [nvarchar](150) NULL,
	[Comment] [nvarchar](max) NULL,
	[Response] [nvarchar](max) NULL,
	[RespondedAt] [datetime2](7) NULL,
	[IsVisible] [bit] NOT NULL,
	[IsVerified] [bit] NOT NULL,
	[LanguageId] [smallint] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Review] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Review_Booking] UNIQUE NONCLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ReviewCriteria] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewCriteria](
	[Id] [smallint] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ReviewCriteria] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ReviewCriteria_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ReviewMedia] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewMedia](
	[Id] [uniqueidentifier] NOT NULL,
	[ReviewId] [uniqueidentifier] NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[SortOrder] [smallint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ReviewMedia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[ReviewRating] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewRating](
	[Id] [uniqueidentifier] NOT NULL,
	[ReviewId] [uniqueidentifier] NOT NULL,
	[CriteriaId] [smallint] NOT NULL,
	[Score] [smallint] NOT NULL,
 CONSTRAINT [PK_ReviewRating] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ReviewRating] UNIQUE NONCLUSTERED 
(
	[ReviewId] ASC,
	[CriteriaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Role] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Role_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Subcategory] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subcategory](
	[Id] [uniqueidentifier] NOT NULL,
	[CategoryId] [uniqueidentifier] NOT NULL,
	[Slug] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IconUrl] [nvarchar](max) NULL,
	[SortOrder] [smallint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Subcategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Subcategory_Slug] UNIQUE NONCLUSTERED 
(
	[Slug] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[SubcategoryTranslation] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubcategoryTranslation](
	[Id] [uniqueidentifier] NOT NULL,
	[SubcategoryId] [uniqueidentifier] NOT NULL,
	[LanguageId] [smallint] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_SubcatTranslation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_SubcatTranslation] UNIQUE NONCLUSTERED 
(
	[SubcategoryId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Tag] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tag](
	[Id] [uniqueidentifier] NOT NULL,
	[Slug] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Tag] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Tag_Slug] UNIQUE NONCLUSTERED 
(
	[Slug] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[TicketCategory] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicketCategory](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[NameEn] [nvarchar](50) NULL,
	[AgeRangeMin] [smallint] NULL,
	[AgeRangeMax] [smallint] NULL,
	[IsActive] [bit] NOT NULL,
	[SortOrder] [smallint] NOT NULL,
 CONSTRAINT [PK_TicketCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[TourItinerary] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TourItinerary](
	[Id] [uniqueidentifier] NOT NULL,
	[AttractionId] [uniqueidentifier] NOT NULL,
	[LanguageId] [smallint] NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Overview] [nvarchar](max) NULL,
	[TotalDistanceKm] [decimal](6, 2) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_TourItinerary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_TourItinerary] UNIQUE NONCLUSTERED 
(
	[AttractionId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[TourStop] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TourStop](
	[Id] [uniqueidentifier] NOT NULL,
	[ItineraryId] [uniqueidentifier] NOT NULL,
	[StopNumber] [smallint] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DurationMinutes] [smallint] NULL,
	[Latitude] [decimal](9, 6) NULL,
	[Longitude] [decimal](9, 6) NULL,
	[AdmissionType] [nvarchar](20) NULL,
 CONSTRAINT [PK_TourStop] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_TourStop] UNIQUE NONCLUSTERED 
(
	[ItineraryId] ASC,
	[StopNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[TourStopMedia] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TourStopMedia](
	[Id] [uniqueidentifier] NOT NULL,
	[StopId] [uniqueidentifier] NOT NULL,
	[MediaTypeId] [smallint] NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[SortOrder] [smallint] NOT NULL,
 CONSTRAINT [PK_TourStopMedia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[UserRole] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[GrantedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_UserRole] UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Users] Fecha de script: 27/4/2026 09:28:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[PasswordHash] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[EmailVerified] [bit] NOT NULL,
	[LastLoginAt] [datetime2](7) NULL,
	[RefreshToken] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
	[DeletedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto: Index [IX_Attraction_Active] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Attraction_Active] ON [dbo].[Attraction]
(
	[IsActive] ASC,
	[IsPublished] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Attraction_Location] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Attraction_Location] ON [dbo].[Attraction]
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Attraction_Rating] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Attraction_Rating] ON [dbo].[Attraction]
(
	[RatingAverage] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_Attraction_Slug] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Attraction_Slug] ON [dbo].[Attraction]
(
	[Slug] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Attraction_Subcat] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Attraction_Subcat] ON [dbo].[Attraction]
(
	[SubcategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_AttrMedia_Main] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_AttrMedia_Main] ON [dbo].[AttractionMedia]
(
	[AttractionId] ASC,
	[IsMain] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_AttrTrans_Lang] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_AttrTrans_Lang] ON [dbo].[AttractionTranslation]
(
	[AttractionId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_AuditLog_ChangedAt] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_AuditLog_ChangedAt] ON [dbo].[AuditLog]
(
	[ChangedAt] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_AuditLog_Table_Record] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_AuditLog_Table_Record] ON [dbo].[AuditLog]
(
	[TableName] ASC,
	[RecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Slot_Available] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Slot_Available] ON [dbo].[AvailabilitySlot]
(
	[ProductId] ASC,
	[SlotDate] ASC,
	[CapacityAvailable] ASC
)
WHERE ([IsActive]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Slot_ProductDate] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Slot_ProductDate] ON [dbo].[AvailabilitySlot]
(
	[ProductId] ASC,
	[SlotDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Booking_CreatedAt] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Booking_CreatedAt] ON [dbo].[Booking]
(
	[CreatedAt] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Booking_SlotId] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Booking_SlotId] ON [dbo].[Booking]
(
	[SlotId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Booking_Status] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Booking_Status] ON [dbo].[Booking]
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Booking_UserId] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Booking_UserId] ON [dbo].[Booking]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Client_UserId] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Client_UserId] ON [dbo].[Client]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Locations_ParentId] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Locations_ParentId] ON [dbo].[Locations]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [IX_Locations_Type] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Locations_Type] ON [dbo].[Locations]
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Payment_BookingId] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Payment_BookingId] ON [dbo].[Payment]
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Review_UserId] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Review_UserId] ON [dbo].[Review]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_Subcategory_Category] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_Subcategory_Category] ON [dbo].[Subcategory]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Objeto: Index [IX_UserRole_UserId] Fecha de script: 27/4/2026 09:28:38 ******/
CREATE NONCLUSTERED INDEX [IX_UserRole_UserId] ON [dbo].[UserRole]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Attraction] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Attraction] ADD  DEFAULT ((0.00)) FOR [RatingAverage]
GO
ALTER TABLE [dbo].[Attraction] ADD  DEFAULT ((0)) FOR [RatingCount]
GO
ALTER TABLE [dbo].[Attraction] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Attraction] ADD  DEFAULT ((0)) FOR [IsPublished]
GO
ALTER TABLE [dbo].[Attraction] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Attraction] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[AttractionLanguage] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AttractionMedia] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AttractionMedia] ADD  DEFAULT ((0)) FOR [IsMain]
GO
ALTER TABLE [dbo].[AttractionMedia] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[AttractionMedia] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[AttractionTranslation] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AudioGuide] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AudioGuide] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[AudioGuide] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[AudioGuide] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[AudioGuideStop] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AuditLog] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AuditLog] ADD  DEFAULT (getutcdate()) FOR [ChangedAt]
GO
ALTER TABLE [dbo].[AvailabilitySlot] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[AvailabilitySlot] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[AvailabilitySlot] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[AvailabilitySlot] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Booking] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Booking] ADD  DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [dbo].[Booking] ADD  DEFAULT ('USD') FOR [CurrencyCode]
GO
ALTER TABLE [dbo].[Booking] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Booking] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[BookingDetail] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[BookingDetail] ADD  DEFAULT ((1)) FOR [Quantity]
GO
ALTER TABLE [dbo].[BookingDetail] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Category] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Category] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[Category] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Category] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Category] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[CategoryTranslation] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Client] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Client] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Client] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[InclusionItem] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[InclusionItem] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Language] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Locations] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Locations] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Locations] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Payment] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Payment] ADD  DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [dbo].[Payment] ADD  DEFAULT ('USD') FOR [CurrencyCode]
GO
ALTER TABLE [dbo].[Payment] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Payment] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[PriceTier] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[PriceTier] ADD  DEFAULT ('USD') FOR [CurrencyCode]
GO
ALTER TABLE [dbo].[PriceTier] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PriceTier] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[PriceTier] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[ProductOption] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ProductOption] ADD  DEFAULT ((24)) FOR [CancelPolicyHours]
GO
ALTER TABLE [dbo].[ProductOption] ADD  DEFAULT ((1)) FOR [MinParticipants]
GO
ALTER TABLE [dbo].[ProductOption] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ProductOption] ADD  DEFAULT ((0)) FOR [IsPrivate]
GO
ALTER TABLE [dbo].[ProductOption] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[ProductOption] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ProductOption] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT ((0)) FOR [Monday]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT ((0)) FOR [Tuesday]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT ((0)) FOR [Wednesday]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT ((0)) FOR [Thursday]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT ((0)) FOR [Friday]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT ((0)) FOR [Saturday]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT ((0)) FOR [Sunday]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT ((20)) FOR [DefaultCapacity]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[ProductScheduleTime] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ProductScheduleTime] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[ProductTranslation] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Review] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Review] ADD  DEFAULT ((1)) FOR [IsVisible]
GO
ALTER TABLE [dbo].[Review] ADD  DEFAULT ((1)) FOR [IsVerified]
GO
ALTER TABLE [dbo].[Review] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Review] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[ReviewMedia] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ReviewMedia] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[ReviewMedia] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewRating] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Subcategory] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Subcategory] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[Subcategory] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Subcategory] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Subcategory] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[SubcategoryTranslation] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Tag] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[TicketCategory] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[TicketCategory] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TicketCategory] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[TourItinerary] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[TourItinerary] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[TourItinerary] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[TourStop] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[TourStopMedia] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[TourStopMedia] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (getutcdate()) FOR [GrantedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [EmailVerified]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Attraction]  WITH CHECK ADD  CONSTRAINT [FK_Attraction_Location] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Locations] ([Id])
GO
ALTER TABLE [dbo].[Attraction] CHECK CONSTRAINT [FK_Attraction_Location]
GO
ALTER TABLE [dbo].[Attraction]  WITH CHECK ADD  CONSTRAINT [FK_Attraction_Subcat] FOREIGN KEY([SubcategoryId])
REFERENCES [dbo].[Subcategory] ([Id])
GO
ALTER TABLE [dbo].[Attraction] CHECK CONSTRAINT [FK_Attraction_Subcat]
GO
ALTER TABLE [dbo].[AttractionInclusion]  WITH CHECK ADD  CONSTRAINT [FK_AttrIncl_Attraction] FOREIGN KEY([AttractionId])
REFERENCES [dbo].[Attraction] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttractionInclusion] CHECK CONSTRAINT [FK_AttrIncl_Attraction]
GO
ALTER TABLE [dbo].[AttractionInclusion]  WITH CHECK ADD  CONSTRAINT [FK_AttrIncl_InclusionItem] FOREIGN KEY([InclusionItemId])
REFERENCES [dbo].[InclusionItem] ([Id])
GO
ALTER TABLE [dbo].[AttractionInclusion] CHECK CONSTRAINT [FK_AttrIncl_InclusionItem]
GO
ALTER TABLE [dbo].[AttractionLanguage]  WITH CHECK ADD  CONSTRAINT [FK_AttrLang_Attraction] FOREIGN KEY([AttractionId])
REFERENCES [dbo].[Attraction] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttractionLanguage] CHECK CONSTRAINT [FK_AttrLang_Attraction]
GO
ALTER TABLE [dbo].[AttractionLanguage]  WITH CHECK ADD  CONSTRAINT [FK_AttrLang_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[AttractionLanguage] CHECK CONSTRAINT [FK_AttrLang_Language]
GO
ALTER TABLE [dbo].[AttractionMedia]  WITH CHECK ADD  CONSTRAINT [FK_AttrMedia_Attraction] FOREIGN KEY([AttractionId])
REFERENCES [dbo].[Attraction] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttractionMedia] CHECK CONSTRAINT [FK_AttrMedia_Attraction]
GO
ALTER TABLE [dbo].[AttractionMedia]  WITH CHECK ADD  CONSTRAINT [FK_AttrMedia_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[AttractionMedia] CHECK CONSTRAINT [FK_AttrMedia_Language]
GO
ALTER TABLE [dbo].[AttractionMedia]  WITH CHECK ADD  CONSTRAINT [FK_AttrMedia_MediaType] FOREIGN KEY([MediaTypeId])
REFERENCES [dbo].[MediaType] ([Id])
GO
ALTER TABLE [dbo].[AttractionMedia] CHECK CONSTRAINT [FK_AttrMedia_MediaType]
GO
ALTER TABLE [dbo].[AttractionTag]  WITH CHECK ADD  CONSTRAINT [FK_AttrTag_Attraction] FOREIGN KEY([AttractionId])
REFERENCES [dbo].[Attraction] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttractionTag] CHECK CONSTRAINT [FK_AttrTag_Attraction]
GO
ALTER TABLE [dbo].[AttractionTag]  WITH CHECK ADD  CONSTRAINT [FK_AttrTag_Tag] FOREIGN KEY([TagId])
REFERENCES [dbo].[Tag] ([Id])
GO
ALTER TABLE [dbo].[AttractionTag] CHECK CONSTRAINT [FK_AttrTag_Tag]
GO
ALTER TABLE [dbo].[AttractionTranslation]  WITH CHECK ADD  CONSTRAINT [FK_AttrTrans_Attraction] FOREIGN KEY([AttractionId])
REFERENCES [dbo].[Attraction] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttractionTranslation] CHECK CONSTRAINT [FK_AttrTrans_Attraction]
GO
ALTER TABLE [dbo].[AttractionTranslation]  WITH CHECK ADD  CONSTRAINT [FK_AttrTrans_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[AttractionTranslation] CHECK CONSTRAINT [FK_AttrTrans_Language]
GO
ALTER TABLE [dbo].[AudioGuide]  WITH CHECK ADD  CONSTRAINT [FK_AudioGuide_Attraction] FOREIGN KEY([AttractionId])
REFERENCES [dbo].[Attraction] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AudioGuide] CHECK CONSTRAINT [FK_AudioGuide_Attraction]
GO
ALTER TABLE [dbo].[AudioGuide]  WITH CHECK ADD  CONSTRAINT [FK_AudioGuide_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[AudioGuide] CHECK CONSTRAINT [FK_AudioGuide_Language]
GO
ALTER TABLE [dbo].[AudioGuideStop]  WITH CHECK ADD  CONSTRAINT [FK_AudioGuideStop_Guide] FOREIGN KEY([AudioGuideId])
REFERENCES [dbo].[AudioGuide] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AudioGuideStop] CHECK CONSTRAINT [FK_AudioGuideStop_Guide]
GO
ALTER TABLE [dbo].[AvailabilitySlot]  WITH CHECK ADD  CONSTRAINT [FK_AvailSlot_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[ProductOption] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AvailabilitySlot] CHECK CONSTRAINT [FK_AvailSlot_Product]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_Language]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Slot] FOREIGN KEY([SlotId])
REFERENCES [dbo].[AvailabilitySlot] ([Id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_Slot]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[BookingStatus] ([Id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_Status]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_Users]
GO
ALTER TABLE [dbo].[BookingDetail]  WITH CHECK ADD  CONSTRAINT [FK_BookingDetail_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BookingDetail] CHECK CONSTRAINT [FK_BookingDetail_Booking]
GO
ALTER TABLE [dbo].[BookingDetail]  WITH CHECK ADD  CONSTRAINT [FK_BookingDetail_PriceTier] FOREIGN KEY([PriceTierId])
REFERENCES [dbo].[PriceTier] ([Id])
GO
ALTER TABLE [dbo].[BookingDetail] CHECK CONSTRAINT [FK_BookingDetail_PriceTier]
GO
ALTER TABLE [dbo].[CategoryTranslation]  WITH CHECK ADD  CONSTRAINT [FK_CatTrans_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CategoryTranslation] CHECK CONSTRAINT [FK_CatTrans_Category]
GO
ALTER TABLE [dbo].[CategoryTranslation]  WITH CHECK ADD  CONSTRAINT [FK_CatTrans_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[CategoryTranslation] CHECK CONSTRAINT [FK_CatTrans_Language]
GO
ALTER TABLE [dbo].[Client]  WITH CHECK ADD  CONSTRAINT [FK_Client_Language] FOREIGN KEY([PreferredLang])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[Client] CHECK CONSTRAINT [FK_Client_Language]
GO
ALTER TABLE [dbo].[Client]  WITH CHECK ADD  CONSTRAINT [FK_Client_Location] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Locations] ([Id])
GO
ALTER TABLE [dbo].[Client] CHECK CONSTRAINT [FK_Client_Location]
GO
ALTER TABLE [dbo].[Client]  WITH CHECK ADD  CONSTRAINT [FK_Client_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Client] CHECK CONSTRAINT [FK_Client_Users]
GO
ALTER TABLE [dbo].[InclusionItem]  WITH CHECK ADD  CONSTRAINT [FK_Inclusion_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[InclusionItem] CHECK CONSTRAINT [FK_Inclusion_Language]
GO
ALTER TABLE [dbo].[Locations]  WITH CHECK ADD  CONSTRAINT [FK_Locations_Parent] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Locations] ([Id])
GO
ALTER TABLE [dbo].[Locations] CHECK CONSTRAINT [FK_Locations_Parent]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_Booking]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Method] FOREIGN KEY([PaymentMethodId])
REFERENCES [dbo].[PaymentMethodType] ([Id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_Method]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Status] FOREIGN KEY([StatusId])
REFERENCES [dbo].[PaymentStatusType] ([Id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_Status]
GO
ALTER TABLE [dbo].[PriceTier]  WITH CHECK ADD  CONSTRAINT [FK_PriceTier_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[ProductOption] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PriceTier] CHECK CONSTRAINT [FK_PriceTier_Product]
GO
ALTER TABLE [dbo].[PriceTier]  WITH CHECK ADD  CONSTRAINT [FK_PriceTier_TicketCat] FOREIGN KEY([TicketCategoryId])
REFERENCES [dbo].[TicketCategory] ([Id])
GO
ALTER TABLE [dbo].[PriceTier] CHECK CONSTRAINT [FK_PriceTier_TicketCat]
GO
ALTER TABLE [dbo].[ProductInclusion]  WITH CHECK ADD  CONSTRAINT [FK_ProdIncl_Inclusion] FOREIGN KEY([InclusionItemId])
REFERENCES [dbo].[InclusionItem] ([Id])
GO
ALTER TABLE [dbo].[ProductInclusion] CHECK CONSTRAINT [FK_ProdIncl_Inclusion]
GO
ALTER TABLE [dbo].[ProductInclusion]  WITH CHECK ADD  CONSTRAINT [FK_ProdIncl_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[ProductOption] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductInclusion] CHECK CONSTRAINT [FK_ProdIncl_Product]
GO
ALTER TABLE [dbo].[ProductOption]  WITH CHECK ADD  CONSTRAINT [FK_ProductOpt_Attraction] FOREIGN KEY([AttractionId])
REFERENCES [dbo].[Attraction] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductOption] CHECK CONSTRAINT [FK_ProductOpt_Attraction]
GO
ALTER TABLE [dbo].[ProductScheduleTemplate]  WITH CHECK ADD  CONSTRAINT [FK_Schedule_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[ProductOption] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductScheduleTemplate] CHECK CONSTRAINT [FK_Schedule_Product]
GO
ALTER TABLE [dbo].[ProductScheduleTime]  WITH CHECK ADD  CONSTRAINT [FK_ScheduleTime_Template] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[ProductScheduleTemplate] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductScheduleTime] CHECK CONSTRAINT [FK_ScheduleTime_Template]
GO
ALTER TABLE [dbo].[ProductTranslation]  WITH CHECK ADD  CONSTRAINT [FK_ProdTrans_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[ProductTranslation] CHECK CONSTRAINT [FK_ProdTrans_Language]
GO
ALTER TABLE [dbo].[ProductTranslation]  WITH CHECK ADD  CONSTRAINT [FK_ProdTrans_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[ProductOption] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductTranslation] CHECK CONSTRAINT [FK_ProdTrans_Product]
GO
ALTER TABLE [dbo].[Review]  WITH CHECK ADD  CONSTRAINT [FK_Review_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO
ALTER TABLE [dbo].[Review] CHECK CONSTRAINT [FK_Review_Booking]
GO
ALTER TABLE [dbo].[Review]  WITH CHECK ADD  CONSTRAINT [FK_Review_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[Review] CHECK CONSTRAINT [FK_Review_Language]
GO
ALTER TABLE [dbo].[Review]  WITH CHECK ADD  CONSTRAINT [FK_Review_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Review] CHECK CONSTRAINT [FK_Review_Users]
GO
ALTER TABLE [dbo].[ReviewMedia]  WITH CHECK ADD  CONSTRAINT [FK_ReviewMedia_Review] FOREIGN KEY([ReviewId])
REFERENCES [dbo].[Review] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewMedia] CHECK CONSTRAINT [FK_ReviewMedia_Review]
GO
ALTER TABLE [dbo].[ReviewRating]  WITH CHECK ADD  CONSTRAINT [FK_ReviewRating_Criteria] FOREIGN KEY([CriteriaId])
REFERENCES [dbo].[ReviewCriteria] ([Id])
GO
ALTER TABLE [dbo].[ReviewRating] CHECK CONSTRAINT [FK_ReviewRating_Criteria]
GO
ALTER TABLE [dbo].[ReviewRating]  WITH CHECK ADD  CONSTRAINT [FK_ReviewRating_Review] FOREIGN KEY([ReviewId])
REFERENCES [dbo].[Review] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewRating] CHECK CONSTRAINT [FK_ReviewRating_Review]
GO
ALTER TABLE [dbo].[Subcategory]  WITH CHECK ADD  CONSTRAINT [FK_Subcategory_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([Id])
GO
ALTER TABLE [dbo].[Subcategory] CHECK CONSTRAINT [FK_Subcategory_Category]
GO
ALTER TABLE [dbo].[SubcategoryTranslation]  WITH CHECK ADD  CONSTRAINT [FK_SubcatTrans_Lang] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[SubcategoryTranslation] CHECK CONSTRAINT [FK_SubcatTrans_Lang]
GO
ALTER TABLE [dbo].[SubcategoryTranslation]  WITH CHECK ADD  CONSTRAINT [FK_SubcatTrans_Sub] FOREIGN KEY([SubcategoryId])
REFERENCES [dbo].[Subcategory] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubcategoryTranslation] CHECK CONSTRAINT [FK_SubcatTrans_Sub]
GO
ALTER TABLE [dbo].[TourItinerary]  WITH CHECK ADD  CONSTRAINT [FK_TourItin_Attraction] FOREIGN KEY([AttractionId])
REFERENCES [dbo].[Attraction] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TourItinerary] CHECK CONSTRAINT [FK_TourItin_Attraction]
GO
ALTER TABLE [dbo].[TourItinerary]  WITH CHECK ADD  CONSTRAINT [FK_TourItin_Language] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Language] ([Id])
GO
ALTER TABLE [dbo].[TourItinerary] CHECK CONSTRAINT [FK_TourItin_Language]
GO
ALTER TABLE [dbo].[TourStop]  WITH CHECK ADD  CONSTRAINT [FK_TourStop_Itinerary] FOREIGN KEY([ItineraryId])
REFERENCES [dbo].[TourItinerary] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TourStop] CHECK CONSTRAINT [FK_TourStop_Itinerary]
GO
ALTER TABLE [dbo].[TourStopMedia]  WITH CHECK ADD  CONSTRAINT [FK_TourStopMedia_Stop] FOREIGN KEY([StopId])
REFERENCES [dbo].[TourStop] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TourStopMedia] CHECK CONSTRAINT [FK_TourStopMedia_Stop]
GO
ALTER TABLE [dbo].[TourStopMedia]  WITH CHECK ADD  CONSTRAINT [FK_TourStopMedia_Type] FOREIGN KEY([MediaTypeId])
REFERENCES [dbo].[MediaType] ([Id])
GO
ALTER TABLE [dbo].[TourStopMedia] CHECK CONSTRAINT [FK_TourStopMedia_Type]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Users]
GO
ALTER TABLE [dbo].[Attraction]  WITH CHECK ADD  CONSTRAINT [CK_Attraction_Difficulty] CHECK  (([DifficultyLevel]='hard' OR [DifficultyLevel]='moderate' OR [DifficultyLevel]='easy'))
GO
ALTER TABLE [dbo].[Attraction] CHECK CONSTRAINT [CK_Attraction_Difficulty]
GO
ALTER TABLE [dbo].[AttractionInclusion]  WITH CHECK ADD  CONSTRAINT [CK_AttrIncl_Type] CHECK  (([Type]='bring' OR [Type]='optional' OR [Type]='not_included' OR [Type]='included'))
GO
ALTER TABLE [dbo].[AttractionInclusion] CHECK CONSTRAINT [CK_AttrIncl_Type]
GO
ALTER TABLE [dbo].[AttractionLanguage]  WITH CHECK ADD  CONSTRAINT [CK_AttrLang_GuideType] CHECK  (([GuideType]='app' OR [GuideType]='written' OR [GuideType]='audio' OR [GuideType]='live'))
GO
ALTER TABLE [dbo].[AttractionLanguage] CHECK CONSTRAINT [CK_AttrLang_GuideType]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD  CONSTRAINT [CK_AuditLog_Action] CHECK  (([Action]='DELETE' OR [Action]='UPDATE' OR [Action]='INSERT'))
GO
ALTER TABLE [dbo].[AuditLog] CHECK CONSTRAINT [CK_AuditLog_Action]
GO
ALTER TABLE [dbo].[AvailabilitySlot]  WITH CHECK ADD  CONSTRAINT [CK_AvailSlot_Capacity] CHECK  (([CapacityAvailable]<=[CapacityTotal] AND [CapacityTotal]>(0) AND [CapacityAvailable]>=(0)))
GO
ALTER TABLE [dbo].[AvailabilitySlot] CHECK CONSTRAINT [CK_AvailSlot_Capacity]
GO
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [CK_Booking_Amount] CHECK  (([TotalAmount]>=(0)))
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [CK_Booking_Amount]
GO
ALTER TABLE [dbo].[BookingDetail]  WITH CHECK ADD  CONSTRAINT [CK_BookingDetail_Qty] CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[BookingDetail] CHECK CONSTRAINT [CK_BookingDetail_Qty]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [CK_Payment_Amount] CHECK  (([Amount]>=(0)))
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [CK_Payment_Amount]
GO
ALTER TABLE [dbo].[PriceTier]  WITH CHECK ADD  CONSTRAINT [CK_PriceTier_Price] CHECK  (([Price]>=(0)))
GO
ALTER TABLE [dbo].[PriceTier] CHECK CONSTRAINT [CK_PriceTier_Price]
GO
ALTER TABLE [dbo].[ProductInclusion]  WITH CHECK ADD  CONSTRAINT [CK_ProdIncl_Type] CHECK  (([Type]='bring' OR [Type]='optional' OR [Type]='not_included' OR [Type]='included'))
GO
ALTER TABLE [dbo].[ProductInclusion] CHECK CONSTRAINT [CK_ProdIncl_Type]
GO
ALTER TABLE [dbo].[Review]  WITH CHECK ADD  CONSTRAINT [CK_Review_Score] CHECK  (([OverallScore]>=(1.00) AND [OverallScore]<=(5.00)))
GO
ALTER TABLE [dbo].[Review] CHECK CONSTRAINT [CK_Review_Score]
GO
ALTER TABLE [dbo].[ReviewRating]  WITH CHECK ADD  CONSTRAINT [CK_ReviewRating_Score] CHECK  (([Score]>=(1) AND [Score]<=(5)))
GO
ALTER TABLE [dbo].[ReviewRating] CHECK CONSTRAINT [CK_ReviewRating_Score]
GO
ALTER TABLE [dbo].[TourStop]  WITH CHECK ADD  CONSTRAINT [CK_TourStop_Admission] CHECK  (([AdmissionType]='excluded' OR [AdmissionType]='optional' OR [AdmissionType]='included'))
GO
ALTER TABLE [dbo].[TourStop] CHECK CONSTRAINT [CK_TourStop_Admission]
GO