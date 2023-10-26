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

        await using (var connection = new SqliteConnection(Source.ConnectionString))
        {
            try
            {
                await connection.OpenAsync();
                await using var command = new SqliteCommand(Source.LoadQuery, connection);
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var key = reader.GetString(0);
                    var value = reader.IsDBNull(1) ? null : reader.GetString(1);

                    if (data.ContainsKey(key))
                    {
                        throw new InvalidOperationException($"Duplicated key '{key}' found in configuration.");
                    }

                    data[key] = value;
                }
            }
            catch (Exception ex) when (ex is SqliteException or InvalidOperationException)
            {
                _logger.LogError(ex, "An error occurred while loading the configuration.");
                throw;
            }

            finally
            {
                await connection.CloseAsync();
            }
        }

        Data = data;
    }

    public async Task SaveAsync(IDictionary<string, string> data)
    {
        await using var connection = new SqliteConnection(Source.ConnectionString);
        await connection.OpenAsync();

        await using var transaction = connection.BeginTransaction();
        try
        {
            foreach (var kvp in data)
            {
                var commandText = Source.SaveCommand;
                await using var command = new SqliteCommand(commandText, connection);
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
    }

    public override void Set(string key, string? value)
    {
        base.Set(key, value);

        if (value != null) SaveAsync(new Dictionary<string, string> {{key, value}}).GetAwaiter().GetResult();
    }
}