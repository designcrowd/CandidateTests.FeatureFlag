using CandidateTests.FeatureFlags.Api.Data;
using CandidateTests.FeatureFlags.Api.Evaluation;
using FluentAssertions;
using Xunit;

namespace CandidateTests.FeatureFlags.Tests.Evaluation;

public class FlagEvaluatorTests
{
    private readonly FlagEvaluator _evaluator = new();

    [Fact]
    public void Evaluate_ShouldReturnTrue_WhenFlagIsEnabledAndFullyRolledOut()
    {
        // Arrange
        var flag = new Flag { Key = "feature-x", Enabled = true, RolloutPercent = 100 };

        // Act
        var result = _evaluator.Evaluate(flag, userId: "user-1");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_ShouldReturnFalse_WhenFlagIsDisabled()
    {
        // Arrange
        var flag = new Flag { Key = "feature-x", Enabled = false, RolloutPercent = 100 };

        // Act
        var result = _evaluator.Evaluate(flag, userId: "user-1");

        // Assert
        result.Should().BeFalse();
    }
}
