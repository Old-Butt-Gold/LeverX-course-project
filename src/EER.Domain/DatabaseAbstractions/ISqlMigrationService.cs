namespace EER.Domain.DatabaseAbstractions;

public interface ISqlMigrationService
{
    void ApplyMigrations();
}
