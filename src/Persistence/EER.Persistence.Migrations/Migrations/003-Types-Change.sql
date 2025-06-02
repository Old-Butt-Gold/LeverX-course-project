ALTER TABLE [Supplies].[Equipment]
ALTER COLUMN PricePerDay decimal(7,2) NOT NULL
GO

ALTER TABLE [Supplies].[Rental]
ALTER COLUMN TotalPrice decimal(10,2) NOT NULL
GO

ALTER TABLE [Supplies].[RentalItem]
ALTER COLUMN ActualPrice decimal(7,2) NOT NULL
GO

CREATE INDEX IX_Equipment_IsModerated ON [Supplies].[Equipment] (IsModerated)
GO
