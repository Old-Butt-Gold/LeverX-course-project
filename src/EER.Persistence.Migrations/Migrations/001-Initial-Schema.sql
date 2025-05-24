CREATE SCHEMA [Identity]
GO

CREATE SCHEMA [Supplies]
GO

CREATE SCHEMA [Critique]
GO

CREATE TABLE [Identity].[User] (
  [Id] uniqueidentifier PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
  [Email] nvarchar(150) NOT NULL,
  [PasswordHash] nvarchar(64) NOT NULL,
  [FullName] nvarchar(150),
  [CreatedAt] datetime2(2) NOT NULL DEFAULT GETUTCDATE(),
  [UserRole] nvarchar(255) NOT NULL CHECK ([UserRole] IN ('Customer', 'Owner', 'Admin'))
)
GO

CREATE UNIQUE INDEX [UQ_User_Email] ON [Identity].[User] ("Email")
GO

CREATE TABLE [Identity].[Office] (
  [Id] integer PRIMARY KEY IDENTITY(1, 1),
  [OwnerId] uniqueidentifier NOT NULL,
  [Address] nvarchar(150) NOT NULL,
  [City] nvarchar(100) NOT NULL,
  [Country] nvarchar(100) NOT NULL,
  [IsActive] BIT NOT NULL DEFAULT (1)
)
GO

CREATE INDEX [FK_Office_OwnerId] ON [Identity].[Office] ("OwnerId")
GO

CREATE INDEX [IX_Office_City_Country] ON [Identity].[Office] ("City", "Country")
GO

CREATE TABLE [Identity].[Favorites] (
  [EquipmentItemId] bigInt,
  [UserId] uniqueIdentifier,
  [AddedAt] datetime2(0) NOT NULL DEFAULT GETUTCDATE(),
  PRIMARY KEY ([UserId], [EquipmentItemId])
)
GO

CREATE INDEX [IX_Favorites_AddedAt] ON [Identity].[Favorites] ("AddedAt")
GO

CREATE TABLE [Supplies].[Category] (
  [Id] integer PRIMARY KEY IDENTITY(1, 1),
  [Name] nvarchar(100) NOT NULL,
  [Description] nvarchar(300) NOT NULL,
  [Slug] nvarchar(100) NOT NULL,
  [TotalEquipment] integer NOT NULL DEFAULT (0)
)
GO

CREATE UNIQUE INDEX [UQ_Category_Slug] ON [Supplies].[Category] ("Slug")
GO

ALTER TABLE [Supplies].[Category]
    ADD CONSTRAINT CK_Category_Slug_Length
    CHECK (LEN(Slug) BETWEEN 3 AND 100);
GO

CREATE TABLE [Supplies].[Equipment] (
  [Id] integer PRIMARY KEY IDENTITY(1, 1),
  [CategoryId] integer NOT NULL,
  [OwnerId] uniqueidentifier NOT NULL,
  [Name] nvarchar(100) NOT NULL,
  [Description] nvarchar(3000) NOT NULL,
  [PricePerDay] decimal(8,2) NOT NULL,
  [AverageRating] decimal(3,2) NOT NULL DEFAULT (0),
  [TotalReviews] integer NOT NULL DEFAULT (0),
  [CreatedAt] datetime2(2) NOT NULL DEFAULT GETUTCDATE(),
  [IsModerated] BIT NOT NULL DEFAULT (0)
)
GO

ALTER TABLE [Supplies].[Equipment]
    ADD CONSTRAINT CK_Equipment_IsModerated
    CHECK ([IsModerated] IN (0, 1));
GO

ALTER TABLE [Supplies].[Equipment]
    ADD CONSTRAINT CK_Equipment_Price CHECK ([PricePerDay] > 0);
GO

CREATE INDEX [IX_Equipment_Name] ON [Supplies].[Equipment] ("Name")
GO

CREATE INDEX [FK_Equipment_OwnerId] ON [Supplies].[Equipment] ("OwnerId")
GO

CREATE INDEX [FK_Equipment_CategoryId] ON [Supplies].[Equipment] ("CategoryId")
GO

CREATE TABLE [Supplies].[EquipmentItem] (
  [Id] bigint PRIMARY KEY IDENTITY(1, 1),
  [EquipmentId] integer NOT NULL,
  [OfficeId] integer,
  [SerialNumber] nvarchar(100) NOT NULL,
  [ItemStatus] nvarchar(255) NOT NULL CHECK ([ItemStatus] IN ('Available', 'InUse', 'UnderMaintenance', 'Retired')) DEFAULT 'Available',
  [MaintenanceDate] date,
  [PurchaseDate] date NOT NULL
)
GO

ALTER TABLE [Supplies].[EquipmentItem]
    ADD CONSTRAINT CK_EquipmentItem_Dates CHECK ([PurchaseDate] < [MaintenanceDate]);
GO

CREATE UNIQUE INDEX [UQ_EquipmentItem_Serial] ON [Supplies].[EquipmentItem] ("SerialNumber", "EquipmentId")
GO

CREATE INDEX [FK_EquipmentItem_OfficeId] ON [Supplies].[EquipmentItem] ("OfficeId")
GO

CREATE INDEX [IX_EquipmentItem_EquipmentId_Status] ON [Supplies].[EquipmentItem] ("EquipmentId", "ItemStatus")
GO

CREATE TABLE [Supplies].[EquipmentImages] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [EquipmentId] integer NOT NULL,
  [DisplayOrder] integer NOT NULL,
  [ImageUrl] nvarchar(500) NOT NULL,
  [CreatedAt] datetime2(0) NOT NULL DEFAULT GETUTCDATE()
)
GO

ALTER TABLE [Supplies].[EquipmentImages]
    ADD CONSTRAINT CK_DisplayOrder CHECK (DisplayOrder >= 0);
GO

CREATE UNIQUE INDEX [UQ_EquipmentImages_Order] ON [Supplies].[EquipmentImages] ("EquipmentId", "DisplayOrder")
GO

CREATE TABLE [Supplies].[Rental] (
  [Id] integer PRIMARY KEY IDENTITY(1, 1),
  [CustomerId] uniqueidentifier NOT NULL,
  [OwnerId] uniqueidentifier NOT NULL,
  [StartDate] datetime2(0) NOT NULL,
  [EndDate] datetime2(0) NOT NULL,
  [TotalPrice] decimal(9,2) NOT NULL,
  [Status] nvarchar(255) NOT NULL CHECK ([Status] IN ('Pending', 'Active', 'Completed', 'Canceled')) DEFAULT 'Pending',
  [CreatedAt] datetime2(2) NOT NULL DEFAULT GETUTCDATE()
)
GO

ALTER TABLE [Supplies].[Rental]
    ADD CONSTRAINT CK_Rental_Dates CHECK ([EndDate] > [StartDate]);
GO

ALTER TABLE [Supplies].[Rental]
    ADD CONSTRAINT CK_Rental_TotalPrice CHECK ([TotalPrice] > 0);
GO

CREATE INDEX [IX_Rental_Dates] ON [Supplies].[Rental] ("StartDate", "EndDate")
GO

CREATE INDEX [FK_Rental_CustomerId] ON [Supplies].[Rental] ("CustomerId")
GO

CREATE INDEX [FK_Rental_OwnerId_Status_CreatedAt] ON [Supplies].[Rental] ("OwnerId", "Status", "CreatedAt")
GO

CREATE TABLE [Supplies].[RentalItem] (
  [RentalId] integer,
  [EquipmentItemId] bigint,
  [ActualPrice] decimal(8,2) NOT NULL,
  PRIMARY KEY ([RentalId], [EquipmentItemId])
)
GO

CREATE INDEX [FK_RentalItem_EquipmentItemId] ON [Supplies].[RentalItem] ("EquipmentItemId")
GO

CREATE TABLE [Critique].[Review] (
  [CustomerId] uniqueidentifier NOT NULL,
  [EquipmentId] integer NOT NULL,
  [Rating] tinyint NOT NULL,
  [Comment] nvarchar(1000),
  [CreatedAt] datetime2(2) NOT NULL DEFAULT GETUTCDATE(),
  PRIMARY KEY ([CustomerId], [EquipmentId])
)
GO

ALTER TABLE [Critique].[Review]
    ADD CONSTRAINT CK_Review_Rating CHECK ([Rating] BETWEEN 1 AND 5);
GO

CREATE INDEX [IX_Review_Rating] ON [Critique].[Review] ("Rating")
GO

CREATE INDEX [IX_Review_CreatedAt] ON [Critique].[Review] ("CreatedAt")
GO

-- Foreign Keys --

ALTER TABLE [Identity].[Office] ADD FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Identity].[Favorites] ADD FOREIGN KEY ([EquipmentItemId]) REFERENCES [Supplies].[EquipmentItem] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Identity].[Favorites] ADD FOREIGN KEY ([UserId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Supplies].[Equipment] ADD FOREIGN KEY ([CategoryId]) REFERENCES [Supplies].[Category] ([Id])
GO

ALTER TABLE [Supplies].[Equipment] ADD FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Supplies].[EquipmentItem] ADD FOREIGN KEY ([EquipmentId]) REFERENCES [Supplies].[Equipment] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Supplies].[EquipmentItem] ADD FOREIGN KEY ([OfficeId]) REFERENCES [Identity].[Office] ([Id]) ON DELETE SET NULL
GO

ALTER TABLE [Supplies].[EquipmentImages] ADD FOREIGN KEY ([EquipmentId]) REFERENCES [Supplies].[Equipment] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Supplies].[Rental] ADD FOREIGN KEY ([CustomerId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Supplies].[Rental] ADD FOREIGN KEY ([OwnerId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Supplies].[RentalItem] ADD FOREIGN KEY ([RentalId]) REFERENCES [Supplies].[Rental] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Supplies].[RentalItem] ADD FOREIGN KEY ([EquipmentItemId]) REFERENCES [Supplies].[EquipmentItem] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Critique].[Review] ADD FOREIGN KEY ([CustomerId]) REFERENCES [Identity].[User] ([Id])
GO

ALTER TABLE [Critique].[Review] ADD FOREIGN KEY ([EquipmentId]) REFERENCES [Supplies].[Equipment] ([Id]) ON DELETE CASCADE
GO
