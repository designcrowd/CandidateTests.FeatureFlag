namespace CandidateTests.FeatureFlags.Api.Data;

public interface IFlagDao
{
    Task<IEnumerable<Flag>> GetAllAsync();

    Task<Flag?> GetByKeyAsync(string key);

    Task<Flag> InsertAsync(Flag flag);

    Task<Flag?> UpdateAsync(string key, Flag input);

    Task<bool> DeleteAsync(string key);
}
