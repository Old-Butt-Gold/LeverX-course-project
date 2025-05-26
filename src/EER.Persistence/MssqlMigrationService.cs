using EER.Domain.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EER.Persistence;

internal sealed class MssqlSqlMigrationService : ISqlMigrationService
{
    private readonly string _connectionString;
    private readonly string _migrationsPath;
    private readonly ILogger<MssqlSqlMigrationService> _logger;

    public MssqlSqlMigrationService(IConfiguration configuration, ILogger<MssqlSqlMigrationService> logger)
    {
        _connectionString = configuration.GetConnectionString("MSConnection")!;

        if (_connectionString is null)
            throw new InvalidOperationException("Connection string not found to configure Migration Service");

        var assemblyLocation = typeof(MssqlSqlMigrationService).Assembly.Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation)!;
        _migrationsPath = Path.Combine(assemblyDirectory, "Migrations");

        _logger = logger;
    }

    public void ApplyMigrations()
    {
        EnsureDatabaseExists();
        ApplySchemaMigrations();
    }

    private void EnsureDatabaseExists()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = "",
        };

        var databaseConnection = builder.ToString();

        using var connection = new SqlConnection(databaseConnection);
        connection.Open();

        var createDbCommand = new SqlCommand(@"
            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'EER')
            BEGIN
                CREATE DATABASE EER;
            END", connection);

        createDbCommand.ExecuteNonQuery();
    }

    private void ApplySchemaMigrations()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        EnsureMigrationHistoryTable(connection);

        var migrationFiles = GetPendingMigrations(connection);

        foreach (var migrationFile in migrationFiles)
        {
            _logger.LogInformation("Applying migration: {MigrationId}", migrationFile.Key);
            ApplyMigration(connection, migrationFile.Key, migrationFile.Value);
        }
    }

    private void ApplyMigration(SqlConnection connection, string fileName, string script)
    {
        var transaction = connection.BeginTransaction();
        try
        {
            var batches = script.Split(["GO"], StringSplitOptions.RemoveEmptyEntries);

            foreach (var batch in batches)
            {
                var cleanedBatch = batch.Trim();
                if (string.IsNullOrWhiteSpace(cleanedBatch)) continue;

                using var command = new SqlCommand(cleanedBatch, connection, transaction);
                command.ExecuteNonQuery();
            }

            using var insertCommand = new SqlCommand(
                "INSERT INTO [dbo].[MigrationHistory] ([MigrationId]) VALUES (@migrationId)", connection, transaction);
            insertCommand.Parameters.AddWithValue("@migrationId", fileName);
            insertCommand.ExecuteNonQuery();

            transaction.Commit();
            _logger.LogInformation("Migration {MigrationId} applied successfully", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply migration {MigrationId}", fileName);
            transaction.Rollback();
            throw;
        }
    }

    private Dictionary<string, string> GetPendingMigrations(SqlConnection connection)
    {
        var appliedMigrations = GetAppliedMigrations(connection);
        _logger.LogInformation("Applied migrations: {AppliedMigrations}", appliedMigrations);

        var migrationFiles = Directory.EnumerateFiles(_migrationsPath, "*.sql")
            .Select(filePath =>
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                return new { FilePath = filePath, MigrationId = fileName };
            })
            .Where(fileInfo => !appliedMigrations.Contains(fileInfo.MigrationId))
            .OrderBy(fileInfo => fileInfo.MigrationId, StringComparer.Ordinal)
            .ToList();

        _logger.LogInformation("Found {PendingMigrationsCount} pending migrations", migrationFiles.Count);

        var pendingMigrations = new Dictionary<string, string>();

        foreach (var migrationFile in migrationFiles)
        {
            var script = File.ReadAllText(migrationFile.FilePath);
            pendingMigrations.Add(migrationFile.MigrationId, script);
        }

        return pendingMigrations;
    }

    private void EnsureMigrationHistoryTable(SqlConnection connection)
    {
        var command = new SqlCommand(@"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MigrationHistory')
            BEGIN
                CREATE TABLE [dbo].[MigrationHistory] (
                    [MigrationId] NVARCHAR(150) PRIMARY KEY
                )
            END", connection);
        command.ExecuteNonQuery();
    }

    private HashSet<string> GetAppliedMigrations(SqlConnection connection)
    {
        var command = new SqlCommand("SELECT [MigrationId] FROM [dbo].[MigrationHistory]", connection);
        using var reader = command.ExecuteReader();

        var migrations = new HashSet<string>();
        while (reader.Read())
        {
            migrations.Add(reader.GetString(0));
        }
        return migrations;
    }
}
