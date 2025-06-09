IF OBJECT_ID('[Supplies].[TRG_PreventDoubleBooking]', 'TR') IS NOT NULL
BEGIN
    DROP TRIGGER [Supplies].[TRG_PreventDoubleBooking];
END;
GO

CREATE TRIGGER [Supplies].[TRG_PreventDoubleBooking]
ON [Supplies].[RentalItem]
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT TOP 1 *
        FROM inserted i
        JOIN Supplies.EquipmentItem ei ON i.EquipmentItemId = ei.Id
        WHERE ei.ItemStatus <> 'Available'
    )
    BEGIN
        THROW 50001, N'One or more equipment items are not available for rental.', 1;
    END

    INSERT INTO Supplies.RentalItem (RentalId, EquipmentItemId, ActualPrice, CreatedBy)
    SELECT RentalId, EquipmentItemId, ActualPrice, CreatedBy
    FROM inserted;
END;
GO
