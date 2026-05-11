using CandidateTests.FeatureFlags.Api.Data;

namespace CandidateTests.FeatureFlags.Api.Evaluation;

public class FlagEvaluator
{
    public bool Evaluate(Flag flag, string userId)
    {
        if (!flag.Enabled)
        {
            return false;
        }

        if (flag.RolloutPercent >= 100)
        {
            return true;
        }

        if (flag.RolloutPercent <= 0)
        {
            return false;
        }

        var bucket = StableBucket(userId, flag.Key);
        return bucket < flag.RolloutPercent;
    }

    private static int StableBucket(string userId, string flagKey)
    {
        var input = $"{userId}:{flagKey}";
        unchecked
        {
            var hash = 2166136261u;
            foreach (var c in input)
            {
                hash ^= c;
                hash *= 16777619u;
            }

            return (int) (hash % 100u);
        }
    }
}