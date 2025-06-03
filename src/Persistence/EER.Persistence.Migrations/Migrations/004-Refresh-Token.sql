CREATE TABLE [Identity].[RefreshToken] (
    [Id] bigint PRIMARY KEY IDENTITY(1,1),
    [UserId] uniqueidentifier NOT NULL,
    [Token] nvarchar(88) NOT NULL,
    [CreatedAt] datetime2(2) NOT NULL DEFAULT GETUTCDATE(),
    [ExpiresAt] datetime2(2) NOT NULL,
    [RevokedAt] datetime2(2) NULL,
);
GO

ALTER TABLE [Identity].[RefreshToken] ADD FOREIGN KEY ([UserId]) REFERENCES [Identity].[User] ([Id]) ON DELETE CASCADE
GO

CREATE UNIQUE INDEX [UQ_RefreshToken_Token] ON [Identity].[RefreshToken] ([Token]);
GO
