namespace EER.Persistence.MongoDB.Settings;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string UserCollection { get; set; } = null!;
    public string OfficeCollection { get; set; } = null!;
    public string EquipmentCollection { get; set; } = null!;
    public string EquipmentItemCollection { get; set; } = null!;
    public string CategoryCollection { get; set; } = null!;
    public string RentalCollection { get; set; } = null!;
    public string ImagesEmbedded { get; set; } = null!;
    public string RefreshTokensEmbedded { get; set; } = null!;
}
