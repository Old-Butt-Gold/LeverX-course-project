CREATE DATABASE EER
GO

USE EER;
GO

CREATE SCHEMA [Identity]
GO

CREATE SCHEMA [Supplies]
GO

CREATE SCHEMA [Critique]
GO

CREATE TABLE [Identity].[User] (
  [Id] uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
  [Email] nvarchar(150) NOT NULL,
  [PasswordHash] nvarchar(MAX) NOT NULL,
  [FullName] nvarchar(150),
  [CreatedAt] datetime2(2) NOT NULL DEFAULT GETUTCDATE(),
  [UserRole] nvarchar(255) NOT NULL CHECK ([UserRole] IN ('Customer', 'Owner', 'Admin'))
)
GO

CREATE UNIQUE INDEX [UQ_User_Email] ON [Identity].[User] ("Email")
GO

CREATE TABLE [Identity].[Location] (
  [Id] bigint PRIMARY KEY IDENTITY,
  [Address] nvarchar(150) NOT NULL,
  [City] nvarchar(100) NOT NULL,
  [State] nvarchar(100),
  [Country] nvarchar(100) NOT NULL,
  [PostalCode] nvarchar(20),
  [OwnerId] uniqueidentifier NOT NULL
)
GO

CREATE INDEX [FK_Location_OwnerId] ON [Identity].[Location] ("OwnerId")
GO

CREATE INDEX [IX_Location_City_Country] ON [Identity].[Location] ("City", "Country")
GO

CREATE TABLE [Identity].[Favorites] (
  [UserId] uniqueidentifier,
  [EquipmentId] bigint,
  [AddedAt] datetime2(0) NOT NULL DEFAULT GETUTCDATE(),
  PRIMARY KEY ([UserId], [EquipmentId])
)
GO

CREATE INDEX [IX_Favorites_AddedAt] ON [Identity].[Favorites] ("AddedAt")
GO

CREATE TABLE [Supplies].[EquipmentLocation] (
  [EquipmentId] bigint NOT NULL,
  [LocationId] bigint NOT NULL,
  [AvailableStock] integer NOT NULL DEFAULT (0),
  PRIMARY KEY ([EquipmentId], [LocationId])
)
GO

CREATE TABLE [Supplies].[Category] (
  [Id] integer PRIMARY KEY IDENTITY,
  [Name] nvarchar(100) NOT NULL,
  [Description] nvarchar(300) NOT NULL,
  [Slug] nvarchar(100) NOT NULL,
  [TotalEquipment] integer NOT NULL DEFAULT (0)
)
GO

ALTER TABLE [Supplies].[Category]
ADD CONSTRAINT CK_Category_Slug_Length 
CHECK (LEN(Slug) BETWEEN 3 AND 100);
GO

CREATE UNIQUE INDEX [UQ_Category_Slug] ON [Supplies].[Category] ("Slug")
GO

CREATE TABLE [Supplies].[Equipment] (
  [Id] bigint PRIMARY KEY IDENTITY,
  [CategoryId] integer NOT NULL,
  [OwnerId] uniqueidentifier NOT NULL,
  [Name] nvarchar(100) NOT NULL,
  [Description] nvarchar(3000) NOT NULL,
  [PricePerDay] decimal(9,2) NOT NULL,
  [CreatedAt] datetime2(2) NOT NULL DEFAULT GETUTCDATE(),
  [AverageRating] decimal(3,2) NOT NULL DEFAULT (0),
  [TotalReviews] integer NOT NULL DEFAULT (0),
  [IsModerated] BIT NOT NULL DEFAULT (0),
)
GO

ALTER TABLE [Supplies].[Equipment]
ADD CONSTRAINT CK_Equipment_IsModerated 
CHECK ([IsModerated] IN (0, 1));
GO

ALTER TABLE [Supplies].[Equipment]
ADD CONSTRAINT CK_Equipment_Price CHECK ([PricePerDay] > 0);
GO

CREATE INDEX [FK_Equipment_OwnerId] ON [Supplies].[Equipment] ("OwnerId")
GO

CREATE INDEX [IX_Equipment_CategoryId_PricePerDay] 
ON [Supplies].[Equipment] ([CategoryId], [PricePerDay])
INCLUDE ([AverageRating]);
GO

CREATE INDEX [IX_Equipment_Name] ON [Supplies].[Equipment] ("Name")
GO

CREATE TABLE [Supplies].[EquipmentImages] (
  [Id] bigint PRIMARY KEY IDENTITY,
  [EquipmentId] bigint NOT NULL,
  [ImageUrl] nvarchar(500) NOT NULL,
  [CreatedAt] datetime2(0) NOT NULL DEFAULT GETUTCDATE(),
  [DisplayOrder] integer NOT NULL
)
GO

ALTER TABLE [Supplies].[EquipmentImages]
ADD CONSTRAINT CK_DisplayOrder CHECK (DisplayOrder >= 0);
GO

CREATE UNIQUE INDEX [UQ_EquipmentImage_EquipmentId_DisplayOrder] ON [Supplies].[EquipmentImages] ("EquipmentId", "DisplayOrder")
GO

CREATE TABLE [Supplies].[Rental] (
  [Id] bigint PRIMARY KEY IDENTITY,
  [StartDate] datetime2(0) NOT NULL,
  [EndDate] datetime2(0) NOT NULL,
  [TotalPrice] decimal(9,2) NOT NULL,
  [Quantity] integer NOT NULL,
  [CreatedAt] datetime2(2) NOT NULL DEFAULT GETUTCDATE(),
  [Status] nvarchar(255) NOT NULL CHECK ([Status] IN ('Pending', 'Active', 'Completed', 'Canceled')) DEFAULT 'Pending',
  [CustomerId] uniqueidentifier NOT NULL,
  [OwnerId] uniqueIdentifier NOT NULL,
  [EquipmentId] bigint NOT NULL,
  [LocationId] bigint
)
GO

ALTER TABLE [Supplies].[Rental]
ADD CONSTRAINT CK_Rental_Dates CHECK ([EndDate] > [StartDate]);
GO

ALTER TABLE [Supplies].[Rental]
ADD CONSTRAINT CK_Rental_Quantity CHECK ([Quantity] > 0),
    CONSTRAINT CK_Rental_TotalPrice CHECK ([TotalPrice] > 0);
GO

CREATE INDEX [IX_Rental_Status_Dates] ON [Supplies].[Rental] ("Status", "StartDate", "EndDate")
GO

CREATE INDEX [FK_Rental_CustomerId] ON [Supplies].[Rental] ("CustomerId")
GO

CREATE INDEX [FK_Rental_OwnerId] ON [Supplies].[Rental] ("OwnerId")
GO

CREATE INDEX [FK_Rental_EquipmentId] ON [Supplies].[Rental] ("EquipmentId")
GO

CREATE INDEX [FK_Rental_LocationId] ON [Supplies].[Rental] ("LocationId")
GO

CREATE TABLE [Critique].[Review] (
  [Id] bigint PRIMARY KEY IDENTITY,
  [Rating] integer NOT NULL,
  [Comment] nvarchar(1500),
  [CreatedAt] datetime2(2) NOT NULL DEFAULT GETUTCDATE(),
  [UpdatedAt] datetime2(2) NOT NULL DEFAULT GETUTCDATE(),
  [EquipmentId] bigint NOT NULL,
  [RentalId] bigint NOT NULL
)
GO

ALTER TABLE [Critique].[Review]
ADD CONSTRAINT CK_Review_Rating CHECK ([Rating] BETWEEN 1 AND 5);
GO

CREATE INDEX [IX_Review_Rating] ON [Critique].[Review] ([Rating]);
GO

CREATE INDEX [FK_Review_EquipmentId] ON [Critique].[Review] ("EquipmentId")
GO

CREATE UNIQUE INDEX [UQ_Review_RentalId] ON [Critique].[Review] ("RentalId")
GO

-- Foreign Keys --

ALTER TABLE [Identity].[Location] ADD FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Identity].[Favorites] ADD FOREIGN KEY ([UserId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Identity].[Favorites] ADD FOREIGN KEY ([EquipmentId]) REFERENCES [Supplies].[Equipment] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Supplies].[EquipmentLocation] ADD FOREIGN KEY ([EquipmentId]) REFERENCES [Supplies].[Equipment] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Supplies].[EquipmentLocation] ADD FOREIGN KEY ([LocationId]) REFERENCES [Identity].[Location] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Supplies].[Equipment] ADD FOREIGN KEY ([CategoryId]) REFERENCES [Supplies].[Category] ([Id]) ON DELETE NO ACTION
GO

ALTER TABLE [Supplies].[Equipment] ADD FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Supplies].[EquipmentImages] ADD FOREIGN KEY ([EquipmentId]) REFERENCES [Supplies].[Equipment] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Supplies].[Rental] ADD FOREIGN KEY ([CustomerId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Supplies].[Rental] ADD FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Supplies].[Rental] ADD FOREIGN KEY ([EquipmentId]) REFERENCES [Supplies].[Equipment] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Supplies].[Rental] ADD FOREIGN KEY ([LocationId]) REFERENCES [Identity].[Location] ([Id]) ON DELETE SET NULL
GO

ALTER TABLE [Critique].[Review] ADD FOREIGN KEY ([EquipmentId]) REFERENCES [Supplies].[Equipment] ([Id])
GO

ALTER TABLE [Critique].[Review] ADD FOREIGN KEY ([RentalId]) REFERENCES [Supplies].[Rental] ([Id]) ON DELETE CASCADE
GO