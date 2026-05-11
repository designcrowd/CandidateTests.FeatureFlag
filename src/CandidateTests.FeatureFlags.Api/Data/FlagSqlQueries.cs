namespace CandidateTests.FeatureFlags.Api.Data;

public static class FlagSqlQueries
{
    public const string SelectAll = @"
        SELECT Id, Key, Name, Enabled, RolloutPercent, CreatedAt, UpdatedAt
        FROM Flags
        ORDER BY Key;";

    public const string SelectByKey = @"
        SELECT Id, Key, Name, Enabled, RolloutPercent, CreatedAt, UpdatedAt
        FROM Flags
        WHERE Key = @Key;";

    public const string Insert = @"
        INSERT INTO Flags (Key, Name, Enabled, RolloutPercent, CreatedAt, UpdatedAt)
        VALUES (@Key, @Name, @Enabled, @RolloutPercent, @CreatedAt, @UpdatedAt)
        RETURNING Id, Key, Name, Enabled, RolloutPercent, CreatedAt, UpdatedAt;";

    public const string UpdateByKey = @"
        UPDATE Flags
        SET Name = @Name,
            Enabled = @Enabled,
            RolloutPercent = @RolloutPercent,
            UpdatedAt = @UpdatedAt
        WHERE Key = @Key
        RETURNING Id, Key, Name, Enabled, RolloutPercent, CreatedAt, UpdatedAt;";

    public const string DeleteByKey = @"DELETE FROM Flags WHERE Key = @Key;";
}
