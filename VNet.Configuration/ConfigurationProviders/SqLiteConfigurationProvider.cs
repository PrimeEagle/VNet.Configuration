using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VNet.Configuration.ConfigurationSources;

// ReSharper disable MemberCanBePrivate.Global


namespace VNet.Configuration.ConfigurationProviders;

public class SqLiteConfigurationProvider : ConfigurationProvider
{
    private readonly ILogger _logger;

    public SqLiteConfigurationSource Source { get; }

    public SqLiteConfigurationProvider(SqLiteConfigurationSource source, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<SqLiteConfigurationProvider>();
        Source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public override void Load() => LoadAsync().GetAwaiter().GetResult();


    private async Task LoadAsync()
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        await using var connection = new SqliteConnection(Source.ConnectionString);
        try
        {
            await connection.OpenAsync();

            const string query = """
                                 
                                             SELECT c.Name, s.Key, s.Value
                                             FROM Settings s
                                             INNER JOIN Categories c ON s.CategoryId = c.Id
                                 """;

            await using var command = new SqliteCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var category = reader.GetString(0);
                var keyName = reader.GetString(1);
                var fullKey = $"{category}:{keyName}";

                var value = reader.IsDBNull(2) ? null : reader.GetString(2);

                if (data.ContainsKey(fullKey))
                {
                    throw new InvalidOperationException($"Duplicated key '{fullKey}' found in configuration.");
                }

                data[fullKey] = value;
            }
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "An error occurred while loading the configuration from SQLite.");
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }

        Data = data;
    }

    public async Task SaveAsync(IDictionary<string, string> data)
    {
        await using var connection = new SqliteConnection(Source.ConnectionString);
        await connection.OpenAsync();
        await using var transaction = (SqliteTransaction)await connection.BeginTransactionAsync();

        try
        {
            foreach (var kvp in data)
            {
                var commandText = Source.SaveCommand;
                await using var command = new SqliteCommand(commandText, connection, transaction);
                command.Parameters.AddWithValue("@Key", kvp.Key);
                command.Parameters.AddWithValue("@Value", kvp.Value);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public override void Set(string key, string? value)
    {
        base.Set(key, value);
        if (value != null) SaveAsync(new Dictionary<string, string> {{key, value}}).GetAwaiter().GetResult();
    }
}