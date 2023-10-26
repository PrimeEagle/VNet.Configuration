using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VNet.Configuration.ConfigurationSources;

// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable IDE0051
#pragma warning disable CA1822


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
        await connection.OpenAsync();

        const string query = """
                             
                                     WITH RECURSIVE CategoryPath(Id, Name, Path) AS (
                                         SELECT Id, Name, Name as Path FROM Categories WHERE ParentCategoryId IS NULL
                                         UNION ALL
                                         SELECT c.Id, c.Name, CONCAT(cp.Path, ':', c.Name)
                                         FROM CategoryPath cp
                                         JOIN Categories c ON cp.Id = c.ParentCategoryId
                                     )
                                     SELECT cp.Path, s.Key, s.Value
                                     FROM Settings s
                                     JOIN Categories c ON s.CategoryId = c.Id
                                     JOIN CategoryPath cp ON c.Id = cp.Id
                             """;

        await using var command = new SqliteCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {

            var categoryPath = reader.GetString(0);
            var keyName = reader.GetString(1);
            var fullKey = $"{categoryPath}:{keyName}";

            var value = reader.IsDBNull(2) ? null : reader.GetString(2);

            if (data.ContainsKey(fullKey))
            {
                throw new InvalidOperationException($"Duplicated key '{fullKey}' found in configuration.");
            }

            data[fullKey] = value;
        }

        await connection.CloseAsync();

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
                var keySegments = kvp.Key.Split(':');
                var settingKey = keySegments.Last();
                var categoryPath = string.Join(':', keySegments.Take(keySegments.Length - 1));
                var categoryId = await ResolveCategoryIdFromPathAsync(connection, categoryPath); // this line uses the method

                const string commandText = """
                                           
                                                   UPDATE Settings
                                                   SET Value = @Value
                                                   WHERE Key = @Key AND CategoryId = @CategoryId
                                           """;

                await using var command = new SqliteCommand(commandText, connection, transaction);
                command.Parameters.AddWithValue("@Key", settingKey);
                command.Parameters.AddWithValue("@Value", kvp.Value);
                command.Parameters.AddWithValue("@CategoryId", categoryId);
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "There was an error saving the configuration.");
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
        if (value != null) SaveAsync(new Dictionary<string, string> { { key, value } }).GetAwaiter().GetResult();
    }

    private async Task<int> ResolveCategoryIdFromPathAsync(SqliteConnection connection, string categoryPath)
    {
        var categories = categoryPath.Split(':');
        int? categoryId = null;

        foreach (var category in categories)
        {
            await using var command = new SqliteCommand("SELECT Id FROM Categories WHERE Name = @Name AND (@ParentId IS NULL OR ParentCategoryId = @ParentId)", connection);
            command.Parameters.AddWithValue("@Name", category);

            if (categoryId.HasValue)
            {
                command.Parameters.AddWithValue("@ParentId", categoryId.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@ParentId", DBNull.Value);
            }

            var result = await command.ExecuteScalarAsync();
            if (result != null)
                categoryId = (int) result;
            else
                throw new InvalidOperationException($"Category '{category}' in path '{categoryPath}' could not be resolved.");
        }

        return categoryId ?? throw new InvalidOperationException("Failed to resolve category path.");
    }


    private async Task<string> ResolveCategoryPathAsync(SqliteConnection connection, int categoryId)
    {
        var pathSegments = new List<string>();

        while (categoryId != 0)
        {
            const string query = "SELECT Name, ParentCategoryId FROM Categories WHERE Id = @Id";
            await using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", categoryId);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                throw new InvalidOperationException("Category not found.");
            }

            pathSegments.Insert(0, reader.GetString(0));
            categoryId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
        }

        return string.Join(":", pathSegments);
    }

}