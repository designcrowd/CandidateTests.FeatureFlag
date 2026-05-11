using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CandidateTests.FeatureFlags.Api.Data;

public class FlagDao : IFlagDao
{
    private readonly string _connectionString;

    public FlagDao(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Flag>> GetAllAsync()
    {
        using (var db = GetConnection())
        {
            return await db.QueryAsync<Flag>(FlagSqlQueries.SelectAll);
        }
    }

    public async Task<Flag?> GetByKeyAsync(string key)
    {
        using (var db = GetConnection())
        {
            return await db.QuerySingleOrDefaultAsync<Flag>(FlagSqlQueries.SelectByKey, new { Key = key });
        }
    }

    public async Task<Flag> InsertAsync(Flag flag)
    {
        using (var db = GetConnection())
        {
            return await db.QuerySingleAsync<Flag>(FlagSqlQueries.Insert, new
            {
                flag.Key,
                flag.Name,
                flag.Enabled,
                flag.RolloutPercent,
                flag.CreatedAt,
                flag.UpdatedAt
            });
        }
    }

    public async Task<Flag?> UpdateAsync(string key, Flag input)
    {
        using (var db = GetConnection())
        {
            return await db.QuerySingleOrDefaultAsync<Flag>(FlagSqlQueries.UpdateByKey, new
            {
                Key = key,
                input.Name,
                input.Enabled,
                input.RolloutPercent,
                input.UpdatedAt
            });
        }
    }

    public async Task<bool> DeleteAsync(string key)
    {
        using (var db = GetConnection())
        {
            var rows = await db.ExecuteAsync(FlagSqlQueries.DeleteByKey, new { Key = key });
            return rows > 0;
        }
    }

    protected IDbConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}