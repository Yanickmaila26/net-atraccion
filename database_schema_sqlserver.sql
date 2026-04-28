-- ======================================================
-- SERVICIO ATRACCION â€” SQL SERVER (T-SQL) v3.0
-- Motor: SQL Server 2019+ / Azure SQL
-- Herramienta: SQL Server Management Studio (SSMS)
-- RevisiÃ³n: 2026-04-19
-- ======================================================

USE master;
GO

-- ======================================================
-- 0. CREAR BASE DE DATOS
-- ======================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ServicioAtraccionDB')
BEGIN
    CREATE DATABASE ServicioAtraccionDB
    COLLATE SQL_Latin1_General_CP1_CI_AS;
END
GO

USE ServicioAtraccionDB;
GO

-- ======================================================
-- 1. MÃ“DULO GEOGRÃFICO
-- ======================================================

CREATE TABLE [dbo].[Locations] (
    [Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]          NVARCHAR(100)    NOT NULL,
    [Type]          NVARCHAR(50)     NOT NULL,
    [ParentId]      UNIQUEIDENTIFIER NULL,
    [CountryCode]   NVARCHAR(2)      NULL,
    [CreatedAt]     DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]     DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Locations]        PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Locations_Parent] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Locations]([Id])
);
GO

-- ======================================================
-- 2. IDIOMAS
-- ======================================================

CREATE TABLE [dbo].[Language] (
    [Id]        SMALLINT        NOT NULL IDENTITY(1,1),
    [IsoCode]   NVARCHAR(5)     NOT NULL,
    [Name]      NVARCHAR(50)    NOT NULL,
    [IsActive]  BIT             NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Language]       PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Language_Iso]   UNIQUE      ([IsoCode])
);
GO

INSERT INTO [dbo].[Language] ([IsoCode], [Name]) VALUES
    ('es', 'EspaÃ±ol'),
    ('en', 'English'),
    ('fr', 'FranÃ§ais'),
    ('pt', 'PortuguÃªs'),
    ('de', 'Deutsch'),
    ('it', 'Italiano'),
    ('zh', 'ä¸­æ–‡'),
    ('ja', 'æ—¥æœ¬èªž');
GO

-- ======================================================
-- 3. RBAC â€” ROLES Y USUARIOS
-- ======================================================

CREATE TABLE [dbo].[Role] (
    [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]        NVARCHAR(50)     NOT NULL,
    [Description] NVARCHAR(MAX)    NULL,
    [CreatedAt]   DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]   DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Role]      PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Role_Name] UNIQUE      ([Name])
);
GO

CREATE TABLE [dbo].[Users] (
    [Id]             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Email]          NVARCHAR(150)    NOT NULL,
    [PasswordHash]   NVARCHAR(MAX)    NOT NULL,
    [IsActive]       BIT              NOT NULL DEFAULT 1,
    [EmailVerified]  BIT              NOT NULL DEFAULT 0,
    [LastLoginAt]    DATETIME2(7)     NULL,
    [RefreshToken]   NVARCHAR(MAX)    NULL,
    [CreatedAt]      DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]      DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt]      DATETIME2(7)     NULL,
    CONSTRAINT [PK_Users]       PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Users_Email] UNIQUE      ([Email])
);
GO

CREATE TABLE [dbo].[UserRole] (
    [Id]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId]    UNIQUEIDENTIFIER NOT NULL,
    [RoleId]    UNIQUEIDENTIFIER NOT NULL,
    [GrantedAt] DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_UserRole]          PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_UserRole]          UNIQUE      ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRole_Users]    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRole_Role]     FOREIGN KEY ([RoleId]) REFERENCES [Role]([Id])
);
GO

-- â”€â”€ SEED DATA: ROLES BÃSICOS â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
INSERT INTO [dbo].[Role] ([Id], [Name], [Description]) VALUES
    ('A1111111-1111-1111-1111-111111111111', 'Admin', 'AdministraciÃ³n total: usuarios, configuraciÃ³n y auditorÃ­a'),
    ('B2222222-2222-2222-2222-222222222222', 'Partner', 'GestiÃ³n de catÃ¡logo propio, precios, disponibilidad, itinerarios y POS'),
    ('C3333333-3333-3333-3333-333333333333', 'Client', 'BÃºsqueda de catÃ¡logo, reserva y proceso de pago de atracciones');
GO

-- â”€â”€ SEED DATA: USUARIOS-- Hash BCrypt real para 'secreto123' generado con BCrypt.Net-Next
DECLARE @BCryptHash NVARCHAR(MAX) = '$2a$11$TVBFSiEsaZ4cKgM.H.lTU.cvHVH4F6y9JxAplLsudLc0F53cPWMVe';

INSERT INTO [dbo].[Users] ([Id], [Email], [PasswordHash], [IsActive]) VALUES
    ('10000000-0000-0000-0000-000000000001', 'yanick_admin@atraccion.com', @BCryptHash, 1),
    ('20000000-0000-0000-0000-000000000002', 'yanick_partner@atraccion.com', @BCryptHash, 1),
    ('30000000-0000-0000-0000-000000000003', 'yanick_client@atraccion.com', @BCryptHash, 1);

INSERT INTO [dbo].[UserRole] ([UserId], [RoleId]) VALUES
    ('10000000-0000-0000-0000-000000000001', 'A1111111-1111-1111-1111-111111111111'),
    ('20000000-0000-0000-0000-000000000002', 'B2222222-2222-2222-2222-222222222222'),
    ('30000000-0000-0000-0000-000000000003', 'C3333333-3333-3333-3333-333333333333');
GO

CREATE TABLE [dbo].[Client] (
    [Id]             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId]         UNIQUEIDENTIFIER NOT NULL,
    [FirstName]      NVARCHAR(100)    NOT NULL,
    [LastName]       NVARCHAR(100)    NOT NULL,
    [Phone]          NVARCHAR(20)     NULL,
    [BirthDate]      DATE             NULL,
    [Nationality]    NVARCHAR(100)    NULL,
    [DocumentType]   NVARCHAR(20)     NULL,
    [DocumentNumber] NVARCHAR(50)     NULL,
    [LocationId]     UNIQUEIDENTIFIER NULL,
    [AvatarUrl]      NVARCHAR(MAX)    NULL,
    [PreferredLang]  SMALLINT         NULL,
    [CreatedAt]      DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]      DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt]      DATETIME2(7)     NULL,
    CONSTRAINT [PK_Client]             PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Client_UserId]      UNIQUE      ([UserId]),
    CONSTRAINT [FK_Client_Users]       FOREIGN KEY ([UserId])    REFERENCES [Users]([Id])    ON DELETE CASCADE,
    CONSTRAINT [FK_Client_Location]    FOREIGN KEY ([LocationId]) REFERENCES [Locations]([Id]),
    CONSTRAINT [FK_Client_Language]    FOREIGN KEY ([PreferredLang]) REFERENCES [Language]([Id])
);
GO

-- ======================================================
-- 4. CATÃLOGO â€” CATEGORÃAS Y TAGS
-- ======================================================

CREATE TABLE [dbo].[Category] (
    [Id]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Slug]      NVARCHAR(100)    NOT NULL,
    [Name]      NVARCHAR(100)    NOT NULL,
    [IconUrl]   NVARCHAR(MAX)    NULL,
    [SortOrder] SMALLINT         NOT NULL DEFAULT 0,
    [IsActive]  BIT              NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Category]       PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Category_Slug]  UNIQUE      ([Slug])
);
GO

CREATE TABLE [dbo].[CategoryTranslation] (
    [Id]         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CategoryId] UNIQUEIDENTIFIER NOT NULL,
    [LanguageId] SMALLINT         NOT NULL,
    [Name]       NVARCHAR(100)    NOT NULL,
    CONSTRAINT [PK_CategoryTranslation]    PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_CategoryTranslation]    UNIQUE      ([CategoryId], [LanguageId]),
    CONSTRAINT [FK_CatTrans_Category]      FOREIGN KEY ([CategoryId]) REFERENCES [Category]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CatTrans_Language]      FOREIGN KEY ([LanguageId]) REFERENCES [Language]([Id])
);
GO

CREATE TABLE [dbo].[Subcategory] (
    [Id]         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CategoryId] UNIQUEIDENTIFIER NOT NULL,
    [Slug]       NVARCHAR(100)    NOT NULL,
    [Name]       NVARCHAR(100)    NOT NULL,
    [IconUrl]    NVARCHAR(MAX)    NULL,
    [SortOrder]  SMALLINT         NOT NULL DEFAULT 0,
    [IsActive]   BIT              NOT NULL DEFAULT 1,
    [CreatedAt]  DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]  DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Subcategory]       PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Subcategory_Slug]  UNIQUE      ([Slug]),
    CONSTRAINT [FK_Subcategory_Category] FOREIGN KEY ([CategoryId]) REFERENCES [Category]([Id])
);
GO

CREATE TABLE [dbo].[SubcategoryTranslation] (
    [Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [SubcategoryId] UNIQUEIDENTIFIER NOT NULL,
    [LanguageId]    SMALLINT         NOT NULL,
    [Name]          NVARCHAR(100)    NOT NULL,
    CONSTRAINT [PK_SubcatTranslation]  PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_SubcatTranslation]  UNIQUE      ([SubcategoryId], [LanguageId]),
    CONSTRAINT [FK_SubcatTrans_Sub]    FOREIGN KEY ([SubcategoryId]) REFERENCES [Subcategory]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SubcatTrans_Lang]   FOREIGN KEY ([LanguageId])    REFERENCES [Language]([Id])
);
GO

CREATE TABLE [dbo].[Tag] (
    [Id]   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Slug] NVARCHAR(50)     NOT NULL,
    [Name] NVARCHAR(50)     NOT NULL,
    CONSTRAINT [PK_Tag]      PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Tag_Slug] UNIQUE      ([Slug])
);
GO

-- ======================================================
-- 5. ATRACCIÃ“N PRINCIPAL
-- ======================================================

CREATE TABLE [dbo].[Attraction] (
    [Id]               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [LocationId]       UNIQUEIDENTIFIER NOT NULL,
    [SubcategoryId]    UNIQUEIDENTIFIER NOT NULL,
    [Slug]             NVARCHAR(200)    NOT NULL,
    [Name]             NVARCHAR(150)    NOT NULL,
    [DescriptionShort] NVARCHAR(255)    NULL,
    [DescriptionFull]  NVARCHAR(MAX)    NULL,
    [Address]          NVARCHAR(MAX)    NULL,
    [Latitude]         DECIMAL(9,6)     NULL,
    [Longitude]        DECIMAL(9,6)     NULL,
    [MeetingPoint]     NVARCHAR(MAX)    NULL,
    [RatingAverage]    DECIMAL(3,2)     NOT NULL DEFAULT 0.00,
    [RatingCount]      INT              NOT NULL DEFAULT 0,
    [MinAge]           SMALLINT         NULL,
    [MaxGroupSize]     SMALLINT         NULL,
    [DifficultyLevel]  NVARCHAR(20)     NULL,
    [IsActive]         BIT              NOT NULL DEFAULT 1,
    [IsPublished]      BIT              NOT NULL DEFAULT 0,
    [ManagedById]      UNIQUEIDENTIFIER NULL,
    [CreatedAt]        DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]        DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt]        DATETIME2(7)     NULL,
    CONSTRAINT [PK_Attraction]             PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Attraction_Slug]        UNIQUE      ([Slug]),
    CONSTRAINT [FK_Attraction_Location]    FOREIGN KEY ([LocationId])    REFERENCES [Locations]([Id]),
    CONSTRAINT [FK_Attraction_Subcat]      FOREIGN KEY ([SubcategoryId]) REFERENCES [Subcategory]([Id]),
    CONSTRAINT [CK_Attraction_Difficulty]  CHECK ([DifficultyLevel] IN ('easy','moderate','hard'))
);
GO

CREATE TABLE [dbo].[AttractionTranslation] (
    [Id]               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AttractionId]     UNIQUEIDENTIFIER NOT NULL,
    [LanguageId]       SMALLINT         NOT NULL,
    [Name]             NVARCHAR(150)    NOT NULL,
    [DescriptionShort] NVARCHAR(255)    NULL,
    [DescriptionFull]  NVARCHAR(MAX)    NULL,
    [MeetingPoint]     NVARCHAR(MAX)    NULL,
    CONSTRAINT [PK_AttractionTranslation]  PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_AttractionTranslation]  UNIQUE      ([AttractionId], [LanguageId]),
    CONSTRAINT [FK_AttrTrans_Attraction]   FOREIGN KEY ([AttractionId]) REFERENCES [Attraction]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AttrTrans_Language]     FOREIGN KEY ([LanguageId])   REFERENCES [Language]([Id])
);
GO

CREATE TABLE [dbo].[AttractionLanguage] (
    [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AttractionId] UNIQUEIDENTIFIER NOT NULL,
    [LanguageId]   SMALLINT         NOT NULL,
    [GuideType]    NVARCHAR(20)     NOT NULL,  -- 'live' | 'audio' | 'written' | 'app'
    CONSTRAINT [PK_AttractionLanguage]    PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_AttractionLanguage]    UNIQUE      ([AttractionId], [LanguageId], [GuideType]),
    CONSTRAINT [FK_AttrLang_Attraction]   FOREIGN KEY ([AttractionId]) REFERENCES [Attraction]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AttrLang_Language]     FOREIGN KEY ([LanguageId])   REFERENCES [Language]([Id]),
    CONSTRAINT [CK_AttrLang_GuideType]    CHECK ([GuideType] IN ('live','audio','written','app'))
);
GO

CREATE TABLE [dbo].[AttractionTag] (
    [AttractionId] UNIQUEIDENTIFIER NOT NULL,
    [TagId]        UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_AttractionTag]        PRIMARY KEY ([AttractionId], [TagId]),
    CONSTRAINT [FK_AttrTag_Attraction]   FOREIGN KEY ([AttractionId]) REFERENCES [Attraction]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AttrTag_Tag]          FOREIGN KEY ([TagId])        REFERENCES [Tag]([Id])
);
GO

-- ======================================================
-- 6. MULTIMEDIA
-- ======================================================

CREATE TABLE [dbo].[MediaType] (
    [Id]   SMALLINT     NOT NULL,
    [Name] NVARCHAR(20) NOT NULL,
    CONSTRAINT [PK_MediaType]      PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_MediaType_Name] UNIQUE      ([Name])
);
GO
INSERT INTO [dbo].[MediaType] ([Id], [Name]) VALUES (1,'image'), (2,'video'), (3,'document');
GO

CREATE TABLE [dbo].[AttractionMedia] (
    [Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AttractionId]  UNIQUEIDENTIFIER NOT NULL,
    [MediaTypeId]   SMALLINT         NOT NULL,
    [Url]           NVARCHAR(MAX)    NOT NULL,
    [ThumbnailUrl]  NVARCHAR(MAX)    NULL,
    [Title]         NVARCHAR(150)    NULL,
    [LanguageId]    SMALLINT         NULL,
    [IsMain]        BIT              NOT NULL DEFAULT 0,
    [SortOrder]     SMALLINT         NOT NULL DEFAULT 0,
    [FileSizeKb]    INT              NULL,
    [DurationSecs]  INT              NULL,
    [CreatedAt]     DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_AttractionMedia]         PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AttrMedia_Attraction]    FOREIGN KEY ([AttractionId]) REFERENCES [Attraction]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AttrMedia_MediaType]     FOREIGN KEY ([MediaTypeId])  REFERENCES [MediaType]([Id]),
    CONSTRAINT [FK_AttrMedia_Language]      FOREIGN KEY ([LanguageId])   REFERENCES [Language]([Id])
);
GO

-- ======================================================
-- 7. AUDIOGUÃA
-- ======================================================

CREATE TABLE [dbo].[AudioGuide] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AttractionId]       UNIQUEIDENTIFIER NOT NULL,
    [LanguageId]         SMALLINT         NOT NULL,
    [Title]              NVARCHAR(150)    NOT NULL,
    [Description]        NVARCHAR(MAX)    NULL,
    [TotalDurationSecs]  INT              NULL,
    [IsActive]           BIT              NOT NULL DEFAULT 1,
    [CreatedAt]          DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]          DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_AudioGuide]           PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_AudioGuide]           UNIQUE      ([AttractionId], [LanguageId]),
    CONSTRAINT [FK_AudioGuide_Attraction] FOREIGN KEY ([AttractionId]) REFERENCES [Attraction]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AudioGuide_Language]   FOREIGN KEY ([LanguageId])   REFERENCES [Language]([Id])
);
GO

CREATE TABLE [dbo].[AudioGuideStop] (
    [Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AudioGuideId]  UNIQUEIDENTIFIER NOT NULL,
    [StopNumber]    SMALLINT         NOT NULL,
    [Title]         NVARCHAR(150)    NOT NULL,
    [Description]   NVARCHAR(MAX)    NULL,
    [AudioUrl]      NVARCHAR(MAX)    NOT NULL,
    [DurationSecs]  INT              NULL,
    [Latitude]      DECIMAL(9,6)     NULL,
    [Longitude]     DECIMAL(9,6)     NULL,
    [ImageUrl]      NVARCHAR(MAX)    NULL,
    CONSTRAINT [PK_AudioGuideStop]         PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_AudioGuideStop]         UNIQUE      ([AudioGuideId], [StopNumber]),
    CONSTRAINT [FK_AudioGuideStop_Guide]   FOREIGN KEY ([AudioGuideId]) REFERENCES [AudioGuide]([Id]) ON DELETE CASCADE
);
GO

-- ======================================================
-- 8. ITINERARIO / RECORRIDO
-- ======================================================

CREATE TABLE [dbo].[TourItinerary] (
    [Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AttractionId]    UNIQUEIDENTIFIER NOT NULL,
    [LanguageId]      SMALLINT         NOT NULL,
    [Title]           NVARCHAR(150)    NOT NULL,
    [Overview]        NVARCHAR(MAX)    NULL,
    [TotalDistanceKm] DECIMAL(6,2)     NULL,
    [CreatedAt]       DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]       DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_TourItinerary]          PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_TourItinerary]          UNIQUE      ([AttractionId], [LanguageId]),
    CONSTRAINT [FK_TourItin_Attraction]    FOREIGN KEY ([AttractionId]) REFERENCES [Attraction]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TourItin_Language]      FOREIGN KEY ([LanguageId])   REFERENCES [Language]([Id])
);
GO

CREATE TABLE [dbo].[TourStop] (
    [Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ItineraryId]     UNIQUEIDENTIFIER NOT NULL,
    [StopNumber]      SMALLINT         NOT NULL,
    [Name]            NVARCHAR(150)    NOT NULL,
    [Description]     NVARCHAR(MAX)    NULL,
    [DurationMinutes] SMALLINT         NULL,
    [Latitude]        DECIMAL(9,6)     NULL,
    [Longitude]       DECIMAL(9,6)     NULL,
    [AdmissionType]   NVARCHAR(20)     NULL,
    CONSTRAINT [PK_TourStop]              PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_TourStop]              UNIQUE      ([ItineraryId], [StopNumber]),
    CONSTRAINT [FK_TourStop_Itinerary]    FOREIGN KEY ([ItineraryId]) REFERENCES [TourItinerary]([Id]) ON DELETE CASCADE,
    CONSTRAINT [CK_TourStop_Admission]    CHECK ([AdmissionType] IN ('included','optional','excluded','bring','not_included'))
);
GO

CREATE TABLE [dbo].[TourStopMedia] (
    [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [StopId]      UNIQUEIDENTIFIER NOT NULL,
    [MediaTypeId] SMALLINT         NOT NULL,
    [Url]         NVARCHAR(MAX)    NOT NULL,
    [SortOrder]   SMALLINT         NOT NULL DEFAULT 0,
    CONSTRAINT [PK_TourStopMedia]       PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TourStopMedia_Stop]  FOREIGN KEY ([StopId])      REFERENCES [TourStop]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TourStopMedia_Type]  FOREIGN KEY ([MediaTypeId]) REFERENCES [MediaType]([Id])
);
GO

-- ======================================================
-- 9. INCLUSIONES / EXCLUSIONES
-- ======================================================

CREATE TABLE [dbo].[InclusionItem] (
    [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [IconSlug]    NVARCHAR(50)     NULL,
    [DefaultText] NVARCHAR(MAX)    NOT NULL,
    [LanguageId]  SMALLINT         NOT NULL,
    [CreatedAt]   DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_InclusionItem]       PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Inclusion_Language]  FOREIGN KEY ([LanguageId]) REFERENCES [Language]([Id])
);
GO

CREATE TABLE [dbo].[AttractionInclusion] (
    [AttractionId]      UNIQUEIDENTIFIER NOT NULL,
    [InclusionItemId]   UNIQUEIDENTIFIER NOT NULL,
    [Type]              NVARCHAR(20)     NOT NULL, -- 'included'|'not_included'|'optional'|'bring'
    CONSTRAINT [PK_AttractionInclusion]       PRIMARY KEY ([AttractionId], [InclusionItemId]),
    CONSTRAINT [FK_AttrIncl_Attraction]       FOREIGN KEY ([AttractionId])    REFERENCES [Attraction]([Id])    ON DELETE CASCADE,
    CONSTRAINT [FK_AttrIncl_InclusionItem]    FOREIGN KEY ([InclusionItemId]) REFERENCES [InclusionItem]([Id]),
    CONSTRAINT [CK_AttrIncl_Type]             CHECK ([Type] IN ('included','not_included','optional','bring'))
);
GO

-- ======================================================
-- 10. MODALIDADES / PRODUCT OPTIONS
-- ======================================================

CREATE TABLE [dbo].[ProductOption] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AttractionId]        UNIQUEIDENTIFIER NOT NULL,
    [Slug]                NVARCHAR(150)    NOT NULL,
    [Title]               NVARCHAR(150)    NOT NULL,
    [Description]         NVARCHAR(MAX)    NULL,
    [DurationMinutes]     INT              NULL,
    [DurationDescription] NVARCHAR(100)    NULL,
    [CancelPolicyHours]   INT              NOT NULL DEFAULT 24,
    [CancelPolicyText]    NVARCHAR(MAX)    NULL,
    [MaxGroupSize]        SMALLINT         NULL,
    [MinParticipants]     SMALLINT         NOT NULL DEFAULT 1,
    [IsActive]            BIT              NOT NULL DEFAULT 1,
    [IsPrivate]           BIT              NOT NULL DEFAULT 0,
    [SortOrder]           SMALLINT         NOT NULL DEFAULT 0,
    [CreatedAt]           DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]           DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_ProductOption]           PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_ProductOption_Slug]      UNIQUE      ([AttractionId], [Slug]),
    CONSTRAINT [FK_ProductOpt_Attraction]   FOREIGN KEY ([AttractionId]) REFERENCES [Attraction]([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[ProductTranslation] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ProductId]          UNIQUEIDENTIFIER NOT NULL,
    [LanguageId]         SMALLINT         NOT NULL,
    [Title]              NVARCHAR(150)    NOT NULL,
    [Description]        NVARCHAR(MAX)    NULL,
    [DurationDescription] NVARCHAR(100)   NULL,
    [CancelPolicyText]   NVARCHAR(MAX)    NULL,
    CONSTRAINT [PK_ProductTranslation]     PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_ProductTranslation]     UNIQUE      ([ProductId], [LanguageId]),
    CONSTRAINT [FK_ProdTrans_Product]      FOREIGN KEY ([ProductId])   REFERENCES [ProductOption]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProdTrans_Language]     FOREIGN KEY ([LanguageId])  REFERENCES [Language]([Id])
);
GO

CREATE TABLE [dbo].[ProductInclusion] (
    [ProductId]        UNIQUEIDENTIFIER NOT NULL,
    [InclusionItemId]  UNIQUEIDENTIFIER NOT NULL,
    [Type]             NVARCHAR(20)     NOT NULL, -- 'included'|'not_included'|'optional'|'bring'
    CONSTRAINT [PK_ProductInclusion]       PRIMARY KEY ([ProductId], [InclusionItemId]),
    CONSTRAINT [FK_ProdIncl_Product]       FOREIGN KEY ([ProductId])       REFERENCES [ProductOption]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProdIncl_Inclusion]     FOREIGN KEY ([InclusionItemId]) REFERENCES [InclusionItem]([Id]),
    CONSTRAINT [CK_ProdIncl_Type]          CHECK ([Type] IN ('included','not_included','optional','bring'))
);
GO

-- ======================================================
-- 11. DISPONIBILIDAD â€” PLANTILLAS DE HORARIO
-- ======================================================

CREATE TABLE [dbo].[ProductScheduleTemplate] (
    [Id]              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ProductId]       UNIQUEIDENTIFIER NOT NULL,
    [Name]            NVARCHAR(100)    NOT NULL,
    [Monday]          BIT              NOT NULL DEFAULT 0,
    [Tuesday]         BIT              NOT NULL DEFAULT 0,
    [Wednesday]       BIT              NOT NULL DEFAULT 0,
    [Thursday]        BIT              NOT NULL DEFAULT 0,
    [Friday]          BIT              NOT NULL DEFAULT 0,
    [Saturday]        BIT              NOT NULL DEFAULT 0,
    [Sunday]          BIT              NOT NULL DEFAULT 0,
    [ValidFrom]       DATE             NOT NULL,
    [ValidTo]         DATE             NULL,
    [DefaultCapacity] SMALLINT         NOT NULL DEFAULT 20,
    [IsActive]        BIT              NOT NULL DEFAULT 1,
    [Notes]           NVARCHAR(MAX)    NULL,
    [CreatedAt]       DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]       DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_ProductScheduleTemplate] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Schedule_Product]        FOREIGN KEY ([ProductId]) REFERENCES [ProductOption]([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[ProductScheduleTime] (
    [Id]               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TemplateId]       UNIQUEIDENTIFIER NOT NULL,
    [StartTime]        TIME(0)          NOT NULL,
    [EndTime]          TIME(0)          NULL,
    [CapacityOverride] SMALLINT         NULL,
    [SortOrder]        SMALLINT         NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ProductScheduleTime]   PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ScheduleTime_Template] FOREIGN KEY ([TemplateId]) REFERENCES [ProductScheduleTemplate]([Id]) ON DELETE CASCADE
);
GO

-- ======================================================
-- 11.1 DISPONIBILIDAD â€” CALENDARIO + HORARIOS (GENERADOS)
-- ======================================================

CREATE TABLE [dbo].[AvailabilitySlot] (
    [Id]                UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ProductId]         UNIQUEIDENTIFIER NOT NULL,
    [SlotDate]          DATE             NOT NULL,
    [StartTime]         TIME             NOT NULL,
    [EndTime]           TIME             NULL,
    [CapacityTotal]     SMALLINT         NOT NULL,
    [CapacityAvailable] SMALLINT         NOT NULL,
    [IsActive]          BIT              NOT NULL DEFAULT 1,
    [Notes]             NVARCHAR(MAX)    NULL,
    [CreatedAt]         DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]         DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_AvailabilitySlot]       PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_AvailabilitySlot]       UNIQUE      ([ProductId], [SlotDate], [StartTime]),
    CONSTRAINT [FK_AvailSlot_Product]      FOREIGN KEY ([ProductId]) REFERENCES [ProductOption]([Id]) ON DELETE CASCADE,
    CONSTRAINT [CK_AvailSlot_Capacity]     CHECK ([CapacityAvailable] <= [CapacityTotal] AND [CapacityTotal] > 0 AND [CapacityAvailable] >= 0)
);
GO

-- ======================================================
-- 12. CATEGORÃAS DE TICKETS Y PRECIOS
-- ======================================================

CREATE TABLE [dbo].[TicketCategory] (
    [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]         NVARCHAR(50)     NOT NULL, -- e.g. "NiÃ±o"
    [NameEn]       NVARCHAR(50)     NULL,     -- e.g. "Child"
    [AgeRangeMin]  SMALLINT         NULL,
    [AgeRangeMax]  SMALLINT         NULL,
    [IsActive]     BIT              NOT NULL DEFAULT 1,
    [SortOrder]    SMALLINT         NOT NULL DEFAULT 0,
    CONSTRAINT [PK_TicketCategory] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[PriceTier] (
    [Id]               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ProductId]        UNIQUEIDENTIFIER NOT NULL,
    [TicketCategoryId] UNIQUEIDENTIFIER NOT NULL,
    [Price]            DECIMAL(12,2)    NOT NULL,
    [CurrencyCode]     NVARCHAR(3)      NOT NULL DEFAULT 'USD',
    [IsActive]         BIT              NOT NULL DEFAULT 1,
    [CreatedAt]        DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]        DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_PriceTier]               PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PriceTier_Product]       FOREIGN KEY ([ProductId]) REFERENCES [ProductOption]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PriceTier_TicketCat]     FOREIGN KEY ([TicketCategoryId]) REFERENCES [TicketCategory]([Id]),
    CONSTRAINT [CK_PriceTier_Price]         CHECK ([Price] >= 0)
);
GO

-- ======================================================
-- 13. RESERVAS
-- ======================================================

CREATE TABLE [dbo].[BookingStatus] (
    [Id]   SMALLINT     NOT NULL,
    [Name] NVARCHAR(20) NOT NULL,
    CONSTRAINT [PK_BookingStatus]      PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_BookingStatus_Name] UNIQUE      ([Name])
);
GO
INSERT INTO [dbo].[BookingStatus] ([Id], [Name]) VALUES
    (1,'Pending'), (2,'Confirmed'), (3,'Completed'), (4,'Cancelled'), (5,'NoShow');
GO

CREATE TABLE [dbo].[Booking] (
    [Id]             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PnrCode]        NVARCHAR(8)      NOT NULL,
    [UserId]         UNIQUEIDENTIFIER NOT NULL,
    [SlotId]         UNIQUEIDENTIFIER NOT NULL,
    [StatusId]       SMALLINT         NOT NULL DEFAULT 1,
    [TotalAmount]    DECIMAL(12,2)    NOT NULL,
    [CurrencyCode]   NVARCHAR(3)      NOT NULL DEFAULT 'USD',
    [LanguageId]     SMALLINT         NULL,
    [Notes]          NVARCHAR(MAX)    NULL,
    [InternalNotes]  NVARCHAR(MAX)    NULL,
    [CreatedAt]      DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]      DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [CancelledAt]    DATETIME2(7)     NULL,
    [CancelReason]   NVARCHAR(MAX)    NULL,
    CONSTRAINT [PK_Booking]            PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Booking_Pnr]        UNIQUE      ([PnrCode]),
    CONSTRAINT [FK_Booking_Users]      FOREIGN KEY ([UserId])    REFERENCES [Users]([Id]),
    CONSTRAINT [FK_Booking_Slot]       FOREIGN KEY ([SlotId])    REFERENCES [AvailabilitySlot]([Id]),
    CONSTRAINT [FK_Booking_Status]     FOREIGN KEY ([StatusId])  REFERENCES [BookingStatus]([Id]),
    CONSTRAINT [FK_Booking_Language]   FOREIGN KEY ([LanguageId]) REFERENCES [Language]([Id]),
    CONSTRAINT [CK_Booking_Amount]     CHECK ([TotalAmount] >= 0)
);
GO

CREATE TABLE [dbo].[BookingDetail] (
    [Id]             UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [BookingId]      UNIQUEIDENTIFIER NOT NULL,
    [PriceTierId]    UNIQUEIDENTIFIER NOT NULL,
    [FirstName]      NVARCHAR(100)    NOT NULL,
    [LastName]       NVARCHAR(100)    NOT NULL,
    [DocumentType]   NVARCHAR(20)     NULL,
    [DocumentNumber] NVARCHAR(50)     NOT NULL,
    [Quantity]       SMALLINT         NOT NULL DEFAULT 1,
    [UnitPrice]      DECIMAL(12,2)    NOT NULL,
    [CreatedAt]      DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_BookingDetail]           PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BookingDetail_Booking]   FOREIGN KEY ([BookingId])   REFERENCES [Booking]([Id])    ON DELETE CASCADE,
    CONSTRAINT [FK_BookingDetail_PriceTier] FOREIGN KEY ([PriceTierId]) REFERENCES [PriceTier]([Id]),
    CONSTRAINT [CK_BookingDetail_Qty]       CHECK ([Quantity] > 0)
);
GO

-- ======================================================
-- 14. PAGOS
-- ======================================================

CREATE TABLE [dbo].[PaymentMethodType] (
    [Id]   SMALLINT     NOT NULL,
    [Name] NVARCHAR(30) NOT NULL,
    CONSTRAINT [PK_PaymentMethodType]      PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_PaymentMethodType_Name] UNIQUE      ([Name])
);
GO
INSERT INTO [dbo].[PaymentMethodType] ([Id], [Name]) VALUES
    (1,'Card'), (2,'Transfer'), (3,'Cash'), (4,'PayPal'), (5,'Crypto');
GO

CREATE TABLE [dbo].[PaymentStatusType] (
    [Id]   SMALLINT     NOT NULL,
    [Name] NVARCHAR(20) NOT NULL,
    CONSTRAINT [PK_PaymentStatusType]      PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_PaymentStatusType_Name] UNIQUE      ([Name])
);
GO
INSERT INTO [dbo].[PaymentStatusType] ([Id], [Name]) VALUES
    (1,'Pending'), (2,'Succeeded'), (3,'Failed'), (4,'Refunded'), (5,'Disputed');
GO

CREATE TABLE [dbo].[Payment] (
    [Id]                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [BookingId]             UNIQUEIDENTIFIER NOT NULL,
    [TransactionExternalId] NVARCHAR(100)    NULL,
    [PaymentMethodId]       SMALLINT         NOT NULL,
    [StatusId]              SMALLINT         NOT NULL DEFAULT 1,
    [Amount]                DECIMAL(12,2)    NOT NULL,
    [CurrencyCode]          NVARCHAR(3)      NOT NULL DEFAULT 'USD',
    [GatewayResponse]       NVARCHAR(MAX)    NULL,   -- JSON del gateway
    [PaidAt]                DATETIME2(7)     NULL,
    [RefundedAt]            DATETIME2(7)     NULL,
    [RefundReason]          NVARCHAR(MAX)    NULL,
    [CreatedAt]             DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]             DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Payment]               PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payment_Booking]       FOREIGN KEY ([BookingId])       REFERENCES [Booking]([Id]),
    CONSTRAINT [FK_Payment_Method]        FOREIGN KEY ([PaymentMethodId]) REFERENCES [PaymentMethodType]([Id]),
    CONSTRAINT [FK_Payment_Status]        FOREIGN KEY ([StatusId])        REFERENCES [PaymentStatusType]([Id]),
    CONSTRAINT [CK_Payment_Amount]        CHECK ([Amount] >= 0)
);
GO

-- ======================================================
-- 15. RESEÃ‘AS
-- ======================================================

CREATE TABLE [dbo].[ReviewCriteria] (
    [Id]   SMALLINT     NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_ReviewCriteria]      PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_ReviewCriteria_Name] UNIQUE      ([Name])
);
GO
INSERT INTO [dbo].[ReviewCriteria] ([Id], [Name]) VALUES
    (1,'Guide'), (2,'Punctuality'), (3,'ValueForMoney'), (4,'Safety'), (5,'Cleanliness'), (6,'Organization');
GO

CREATE TABLE [dbo].[Review] (
    [Id]           UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [BookingId]    UNIQUEIDENTIFIER NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [OverallScore] DECIMAL(3,2)     NOT NULL,
    [Title]        NVARCHAR(150)    NULL,
    [Comment]      NVARCHAR(MAX)    NULL,
    [Response]     NVARCHAR(MAX)    NULL,
    [RespondedAt]  DATETIME2(7)     NULL,
    [IsVisible]    BIT              NOT NULL DEFAULT 1,
    [IsVerified]   BIT              NOT NULL DEFAULT 1,
    [LanguageId]   SMALLINT         NULL,
    [CreatedAt]    DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]    DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Review]           PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Review_Booking]   UNIQUE      ([BookingId]),
    CONSTRAINT [FK_Review_Booking]   FOREIGN KEY ([BookingId]) REFERENCES [Booking]([Id]),
    CONSTRAINT [FK_Review_Users]     FOREIGN KEY ([UserId])    REFERENCES [Users]([Id]),
    CONSTRAINT [FK_Review_Language]  FOREIGN KEY ([LanguageId]) REFERENCES [Language]([Id]),
    CONSTRAINT [CK_Review_Score]     CHECK ([OverallScore] BETWEEN 1.00 AND 5.00)
);
GO

CREATE TABLE [dbo].[ReviewRating] (
    [Id]         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ReviewId]   UNIQUEIDENTIFIER NOT NULL,
    [CriteriaId] SMALLINT         NOT NULL,
    [Score]      SMALLINT         NOT NULL,
    CONSTRAINT [PK_ReviewRating]          PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_ReviewRating]          UNIQUE      ([ReviewId], [CriteriaId]),
    CONSTRAINT [FK_ReviewRating_Review]   FOREIGN KEY ([ReviewId])   REFERENCES [Review]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ReviewRating_Criteria] FOREIGN KEY ([CriteriaId]) REFERENCES [ReviewCriteria]([Id]),
    CONSTRAINT [CK_ReviewRating_Score]    CHECK ([Score] BETWEEN 1 AND 5)
);
GO

CREATE TABLE [dbo].[ReviewMedia] (
    [Id]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ReviewId]  UNIQUEIDENTIFIER NOT NULL,
    [Url]       NVARCHAR(MAX)    NOT NULL,
    [SortOrder] SMALLINT         NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_ReviewMedia]        PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ReviewMedia_Review] FOREIGN KEY ([ReviewId]) REFERENCES [Review]([Id]) ON DELETE CASCADE
);
GO

-- ======================================================
-- 16. TABLA DE AUDITORÃA
-- ======================================================

CREATE TABLE [dbo].[AuditLog] (
    [Id]         UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TableName]  NVARCHAR(100)    NOT NULL,
    [RecordId]   UNIQUEIDENTIFIER NOT NULL,
    [Action]     NVARCHAR(10)     NOT NULL,
    [ChangedBy]  NVARCHAR(256)    NULL,           -- Email o ID del usuario
    [ChangedAt]  DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [OldValues]  NVARCHAR(MAX)    NULL,           -- JSON
    [NewValues]  NVARCHAR(MAX)    NULL,           -- JSON
    [IpAddress]  NVARCHAR(45)     NULL,
    [UserAgent]  NVARCHAR(500)    NULL,
    [Endpoint]   NVARCHAR(255)    NULL,
    CONSTRAINT [PK_AuditLog]        PRIMARY KEY ([Id]),
    CONSTRAINT [CK_AuditLog_Action] CHECK ([Action] IN ('INSERT','UPDATE','DELETE'))
);
GO

-- ======================================================
-- 17. ÃNDICES DE RENDIMIENTO
-- ======================================================

CREATE INDEX [IX_Locations_ParentId]    ON [dbo].[Locations]        ([ParentId]);
CREATE INDEX [IX_Locations_Type]        ON [dbo].[Locations]        ([Type]);
CREATE INDEX [IX_Client_UserId]         ON [dbo].[Client]           ([UserId]);
CREATE INDEX [IX_UserRole_UserId]       ON [dbo].[UserRole]         ([UserId]);
CREATE INDEX [IX_Subcategory_Category]  ON [dbo].[Subcategory]      ([CategoryId]);
CREATE INDEX [IX_Attraction_Location]   ON [dbo].[Attraction]       ([LocationId]);
CREATE INDEX [IX_Attraction_Subcat]     ON [dbo].[Attraction]       ([SubcategoryId]);
CREATE INDEX [IX_Attraction_Rating]     ON [dbo].[Attraction]       ([RatingAverage] DESC);
CREATE INDEX [IX_Attraction_Slug]       ON [dbo].[Attraction]       ([Slug]);
CREATE INDEX [IX_Attraction_Active]     ON [dbo].[Attraction]       ([IsActive], [IsPublished]);
CREATE INDEX [IX_AttrTrans_Lang]        ON [dbo].[AttractionTranslation] ([AttractionId], [LanguageId]);
CREATE INDEX [IX_AttrMedia_Main]        ON [dbo].[AttractionMedia]  ([AttractionId], [IsMain]);
CREATE INDEX [IX_Slot_ProductDate]      ON [dbo].[AvailabilitySlot] ([ProductId], [SlotDate]);
CREATE INDEX [IX_Slot_Available]        ON [dbo].[AvailabilitySlot] ([ProductId], [SlotDate], [CapacityAvailable]) WHERE [IsActive] = 1;
CREATE INDEX [IX_Booking_UserId]        ON [dbo].[Booking]          ([UserId]);
CREATE INDEX [IX_Booking_SlotId]        ON [dbo].[Booking]          ([SlotId]);
CREATE INDEX [IX_Booking_Status]        ON [dbo].[Booking]          ([StatusId]);
CREATE INDEX [IX_Booking_CreatedAt]     ON [dbo].[Booking]          ([CreatedAt] DESC);
CREATE INDEX [IX_Payment_BookingId]     ON [dbo].[Payment]          ([BookingId]);
CREATE INDEX [IX_Review_UserId]         ON [dbo].[Review]           ([UserId]);
CREATE INDEX [IX_AuditLog_Table_Record] ON [dbo].[AuditLog]         ([TableName], [RecordId]);
CREATE INDEX [IX_AuditLog_ChangedAt]    ON [dbo].[AuditLog]         ([ChangedAt] DESC);
GO

-- ======================================================
-- 18. DATOS SEMILLA
-- ======================================================

INSERT INTO [dbo].[Locations] ([Id], [Name], [Type], [CountryCode])
VALUES (NEWID(), 'Ecuador', 'country', 'EC');

INSERT INTO [dbo].[Category] ([Slug], [Name], [SortOrder]) VALUES
    ('tours-culturales',    'Tours Culturales',        1),
    ('aventura',            'Aventura y Deporte',      2),
    ('gastronomia',         'GastronomÃ­a',             3),
    ('naturaleza',          'Naturaleza y Ecoturismo', 4),
    ('city-tours',          'City Tours',              5);
GO

-- ======================================================
-- 19. USUARIOS DE BASE DE DATOS POR ROL
-- ======================================================
-- Se crean logins SQL Server y usuarios de BD con permisos
-- mÃ­nimos necesarios para cada rol del sistema.
-- ======================================================

-- â”€â”€ 19.1  LOGIN / USUARIO: yanick_admin â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
-- Acceso completo a la BD (usado solo por el proceso de migraciÃ³n y admin)
CREATE LOGIN [yanick_admin]
    WITH PASSWORD = N'Admin@Atrac2026!',
    DEFAULT_DATABASE = [ServicioAtraccionDB],
    CHECK_EXPIRATION = OFF,
    CHECK_POLICY = ON;
GO

CREATE USER [yanick_admin] FOR LOGIN [yanick_admin];
GO

-- Se le otorga db_datareader + db_datawriter + permiso para ejecutar
ALTER ROLE [db_datareader] ADD MEMBER [yanick_admin];
ALTER ROLE [db_datawriter] ADD MEMBER [yanick_admin];
-- Permiso de DDL solo en esquema dbo (para migraciones)
GRANT ALTER, EXECUTE ON SCHEMA::[dbo] TO [yanick_admin];
GO

-- â”€â”€ 19.2  LOGIN / USUARIO: yanick_api â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
-- Cuenta de servicio usada por la API .NET (solo DML, sin DDL)
CREATE LOGIN [yanick_api]
    WITH PASSWORD = N'Api@Atrac2026!',
    DEFAULT_DATABASE = [ServicioAtraccionDB],
    CHECK_EXPIRATION = OFF,
    CHECK_POLICY = ON;
GO

CREATE USER [yanick_api] FOR LOGIN [yanick_api];
GO

ALTER ROLE [db_datareader] ADD MEMBER [yanick_api];
ALTER ROLE [db_datawriter] ADD MEMBER [yanick_api];
-- Solo puede ejecutar SPs / funciones, NO puede DROP/CREATE
GRANT EXECUTE ON SCHEMA::[dbo] TO [yanick_api];
GO

-- â”€â”€ 19.3  LOGIN / USUARIO: yanick_readonly â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
-- Solo lectura â€” usado por reportes, dashboards, APIs pÃºblicas de consulta
CREATE LOGIN [yanick_readonly]
    WITH PASSWORD = N'ReadOnly@Atrac2026!',
    DEFAULT_DATABASE = [ServicioAtraccionDB],
    CHECK_EXPIRATION = OFF,
    CHECK_POLICY = ON;
GO

CREATE USER [yanick_readonly] FOR LOGIN [yanick_readonly];
GO

ALTER ROLE [db_datareader] ADD MEMBER [yanick_readonly];
-- Solo puede SELECT en las tablas de catÃ¡logo y atracciones
-- NO tiene acceso a datos sensibles: Users, AuditLog, Payment
GRANT SELECT ON [dbo].[Users]    TO [yanick_readonly];
GRANT SELECT ON [dbo].[AuditLog] TO [yanick_readonly];
GRANT SELECT ON [dbo].[Payment]  TO [yanick_readonly];
GO

-- â”€â”€ 19.4  LOGIN / USUARIO: yanick_booking â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
-- Cuenta usada por el proceso de reservas y pagos (Flujo de Cliente)
CREATE LOGIN [yanick_booking]
    WITH PASSWORD = N'Booking@Atrac2026!',
    DEFAULT_DATABASE = [ServicioAtraccionDB],
    CHECK_EXPIRATION = OFF,
    CHECK_POLICY = ON;
GO

CREATE USER [yanick_booking] FOR LOGIN [yanick_booking];
GO

-- Solo puede SELECT/INSERT/UPDATE en tablas transaccionales
GRANT SELECT, INSERT, UPDATE ON [dbo].[Booking]          TO [yanick_booking];
GRANT SELECT, INSERT, UPDATE ON [dbo].[BookingDetail]    TO [yanick_booking];
GRANT SELECT, INSERT, UPDATE ON [dbo].[Payment]          TO [yanick_booking];
GRANT SELECT, UPDATE         ON [dbo].[AvailabilitySlot] TO [yanick_booking];
GRANT SELECT                 ON [dbo].[BookingStatus]    TO [yanick_booking];
GRANT SELECT                 ON [dbo].[PriceTier]        TO [yanick_booking];
GRANT SELECT                 ON [dbo].[ProductOption]    TO [yanick_booking];
GRANT SELECT                 ON [dbo].[Users]            TO [yanick_booking];
GRANT SELECT                 ON [dbo].[Client]           TO [yanick_booking];
GRANT INSERT                 ON [dbo].[AuditLog]         TO [yanick_booking];
GO

-- â”€â”€ 19.5  LOGIN / USUARIO: yanick_partner â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
-- Cuenta para el portal de Partners que gestionan sus atracciones y POS
CREATE LOGIN [yanick_partner]
    WITH PASSWORD = N'Partner@Atrac2026!',
    DEFAULT_DATABASE = [ServicioAtraccionDB],
    CHECK_EXPIRATION = OFF,
    CHECK_POLICY = ON;
GO

CREATE USER [yanick_partner] FOR LOGIN [yanick_partner];
GO

GRANT SELECT, INSERT, UPDATE ON [dbo].[Attraction]            TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[AttractionTranslation] TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[AttractionMedia]       TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[AttractionLanguage]    TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[AttractionInclusion]   TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[AudioGuide]            TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[TourItinerary]         TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[TourStop]              TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[ProductOption]         TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[ProductTranslation]    TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[ProductInclusion]      TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[AvailabilitySlot]      TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[PriceTier]             TO [yanick_partner];
GRANT SELECT, INSERT, DELETE ON [dbo].[AttractionTag]         TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[ProductScheduleTemplate] TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[ProductScheduleTime]     TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[TourStopMedia]          TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[AudioGuideStop]         TO [yanick_partner];
GRANT SELECT, INSERT, UPDATE ON [dbo].[Booking]               TO [yanick_partner]; -- Lectura y Creación (POS)
GRANT SELECT, INSERT         ON [dbo].[BookingDetail]         TO [yanick_partner]; -- Detalle de reservas POS
GRANT SELECT                 ON [dbo].[Review]                TO [yanick_partner];
GRANT UPDATE                 ON [dbo].[Review]                TO [yanick_partner]; -- Para responder reseÃ±as
GRANT SELECT, INSERT         ON [dbo].[Payment]               TO [yanick_partner]; -- Permitir Punto de Venta (POS)
GRANT INSERT                 ON [dbo].[AuditLog]              TO [yanick_partner];

-- Sin acceso a: Users, Client (datos sensibles)
GRANT SELECT ON [dbo].[Users]    TO [yanick_partner];
GRANT SELECT ON [dbo].[Client]   TO [yanick_partner];
GRANT SELECT ON [dbo].[AuditLog] TO [yanick_partner];
GO

-- â”€â”€ RESUMEN DE USUARIOS ACTUALIZADOS â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
-- yanick_admin   â†’ AdministraciÃ³n total + DDL para migraciones
-- yanick_api     â†’ Cuenta de servicio principal de la API .NET
-- yanick_readonly â†’ Solo lectura (reportes, API pÃºblica catÃ¡logo)
-- yanick_booking  â†’ Proceso de reservas y pagos (Cliente)
-- yanick_partner  â†’ Portal de partners (gestiÃ³n + POS)
-- â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

-- ======================================================
-- FIN DEL SCRIPT
-- ======================================================


-- Actualización de permisos para Partner (Dashboard y POS)
GRANT SELECT ON [dbo].[Category] TO [yanick_partner];
GRANT SELECT ON [dbo].[Subcategory] TO [yanick_partner];
GRANT SELECT ON [dbo].[Locations] TO [yanick_partner];
GRANT SELECT ON [dbo].[TicketCategory] TO [yanick_partner];
GRANT SELECT ON [dbo].[Language] TO [yanick_partner];
GRANT SELECT ON [dbo].[Tag] TO [yanick_partner];
GRANT SELECT ON [dbo].[BookingStatus] TO [yanick_partner];
GO
GRANT SELECT ON [dbo].[InclusionItem] TO [yanick_partner];
GRANT SELECT ON [dbo].[MediaType] TO [yanick_partner];
GO
