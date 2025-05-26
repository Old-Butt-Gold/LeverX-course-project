CREATE TRIGGER [Supplies].[TRG_Equipment_AfterInsert]
ON [Supplies].[Equipment]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE c
    SET TotalEquipment = TotalEquipment + 1
    FROM [Supplies].[Category] c
    INNER JOIN inserted i ON c.Id = i.CategoryId;
END;
GO

CREATE TRIGGER [Supplies].[TRG_Equipment_AfterDelete]
ON [Supplies].[Equipment]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE c
    SET TotalEquipment = TotalEquipment - 1
    FROM [Supplies].[Category] c
    INNER JOIN deleted d ON c.Id = d.CategoryId;
END;
GO

CREATE TRIGGER [Supplies].[TRG_Equipment_AfterUpdate]
ON [Supplies].[Equipment]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE c
    SET TotalEquipment = TotalEquipment - 1
    FROM [Supplies].[Category] c
    INNER JOIN deleted d ON c.Id = d.CategoryId;

    UPDATE c
    SET TotalEquipment = TotalEquipment + 1
    FROM [Supplies].[Category] c
    INNER JOIN inserted i ON c.Id = i.CategoryId;
END;
GO

CREATE TRIGGER [Critique].[TRG_Review_AfterInsert]
ON [Critique].[Review]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE e
    SET
        TotalReviews = TotalReviews + 1,
        AverageRating = (
            (TotalReviews * AverageRating + i.Rating) 
            / (TotalReviews + 1)
        )
    FROM [Supplies].[Equipment] e
    INNER JOIN inserted i ON e.Id = i.EquipmentId;
END;
GO

CREATE TRIGGER [Critique].[TRG_Review_AfterDelete]
ON [Critique].[Review]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

	WITH AggregatedData AS (
        SELECT 
            d.EquipmentId,
            COUNT(*) AS RemovedReviews,
            SUM(CAST(d.Rating AS DECIMAL(10,2))) AS RemovedRating
        FROM deleted d
        GROUP BY d.EquipmentId
    )
	UPDATE e
    SET 
        TotalReviews = e.TotalReviews - ad.RemovedReviews,
        AverageRating = 
            CASE 
                WHEN (e.TotalReviews - ad.RemovedReviews) <= 0 THEN 0
                ELSE (e.TotalReviews * e.AverageRating - ad.RemovedRating) 
                     / (e.TotalReviews - ad.RemovedReviews)
            END
    FROM [Supplies].[Equipment] e
    INNER JOIN AggregatedData ad ON e.Id = ad.EquipmentId;
END;
GO

CREATE TRIGGER [Critique].[TRG_Review_AfterUpdate]
ON [Critique].[Review]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

	IF UPDATE(RATING)
	BEGIN
		UPDATE e
        SET 
            AverageRating = (
                (TotalReviews * AverageRating - d.Rating + i.Rating) / 
                TotalReviews
            )
        FROM [Supplies].[Equipment] e
        INNER JOIN inserted i ON e.Id = i.EquipmentId
        INNER JOIN deleted d ON i.CustomerId = d.CustomerId 
            AND i.EquipmentId = d.EquipmentId;
	END;
END;
GO

CREATE PROCEDURE [Supplies].[SPR_HandleRentalStatusChange]
    @RentalId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Status NVARCHAR(255);
    SELECT @Status = Status FROM [Supplies].[Rental] WHERE Id = @RentalId;

    IF @Status IN ('Canceled', 'Completed')
    BEGIN
        UPDATE ei
        SET ItemStatus = 'Available'
        FROM [Supplies].[EquipmentItem] ei
        INNER JOIN [Supplies].[RentalItem] ri ON ei.Id = ri.EquipmentItemId
        WHERE ri.RentalId = @RentalId;
    END;
END;
GO

CREATE TRIGGER [Supplies].[TRG_RentalStatus_AfterUpdate]
ON [Supplies].[Rental]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(Status)
    BEGIN
        DECLARE @RentalId INT;
        SELECT @RentalId = Id FROM inserted;
        EXEC [Supplies].[SPR_HandleRentalStatusChange] @RentalId;
    END;
END;
GO

CREATE TRIGGER [Supplies].[TRG_PreventDoubleBooking]
ON [Supplies].[RentalItem]
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS(
        SELECT TOP 1 *
        FROM inserted i
        JOIN Supplies.EquipmentItem ei ON i.EquipmentItemId = ei.Id
        WHERE ei.ItemStatus <> 'Available'
    )
    BEGIN
        ;THROW 50001, N'One or more equipment items are not available for rental.', 1;
    END
    ELSE
    BEGIN
        INSERT INTO Supplies.RentalItem (RentalId, EquipmentItemId, ActualPrice)
        SELECT RentalId, EquipmentItemId, ActualPrice FROM inserted;
    END
END;
GO
