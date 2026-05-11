using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using CandidateTests.FeatureFlags.Api.Data;
using FluentAssertions;
using Xunit;

namespace CandidateTests.FeatureFlags.Tests.Controllers;

public class FlagsControllerTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly Fixture _fixture = new();

    public FlagsControllerTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetFlags_ShouldReturnSeededFlags_WhenDatabaseIsSeeded()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var flags = await client.GetFromJsonAsync<List<Flag>>("/flags");

        // Assert
        flags.Should().NotBeNull();
        flags!.Should().NotBeEmpty();
        flags!.Should().Contain(f => f.Key == "new-checkout-flow");
    }

    [Fact]
    public async Task GetFlag_ShouldReturnFlag_WhenKeyExists()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var flag = await client.GetFromJsonAsync<Flag>("/flags/dark-mode");

        // Assert
        flag.Should().NotBeNull();
        flag!.Key.Should().Be("dark-mode");
    }

    [Fact]
    public async Task GetFlag_ShouldReturnNotFound_WhenKeyDoesNotExist()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/flags/does-not-exist");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateFlag_ShouldPersistAndReturnCreated_WhenInputIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var input = AnonymousFlag(key: UniqueKey("create"), rolloutPercent: 50);

        // Act
        var response = await client.PostAsJsonAsync("/flags", input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<Flag>();
        created.Should().NotBeNull();
        created!.Key.Should().Be(input.Key);
        created!.Name.Should().Be(input.Name);
        created!.Enabled.Should().Be(input.Enabled);
        created!.RolloutPercent.Should().Be(input.RolloutPercent);
        created!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateFlag_ShouldPersistChanges_WhenKeyExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        var key = UniqueKey("update");
        await client.PostAsJsonAsync("/flags", AnonymousFlag(key, rolloutPercent: 100));
        var update = AnonymousFlag(key, rolloutPercent: 25);

        // Act
        var response = await client.PutAsJsonAsync($"/flags/{key}", update);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<Flag>();
        updated!.Name.Should().Be(update.Name);
        updated!.Enabled.Should().Be(update.Enabled);
        updated!.RolloutPercent.Should().Be(25);
    }

    [Fact]
    public async Task DeleteFlag_ShouldRemoveFlag_WhenKeyExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        var key = UniqueKey("delete");
        await client.PostAsJsonAsync("/flags", AnonymousFlag(key, rolloutPercent: 100));

        // Act
        var response = await client.DeleteAsync($"/flags/{key}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var followup = await client.GetAsync($"/flags/{key}");
        followup.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Evaluate_ShouldReturnEnabled_WhenFlagExistsAndIsActive()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetFromJsonAsync<JsonElement>("/evaluate?userId=user-1&flagKey=new-checkout-flow");

        // Assert
        response.GetProperty("enabled").GetBoolean().Should().BeTrue();
    }

    private static string UniqueKey(string prefix) => $"test-{prefix}-{Guid.NewGuid():N}";

    /// <summary>
    /// Builds a Flag with anonymous Name/Enabled values via AutoFixture, while pinning
    /// the fields the test actually cares about (Key, RolloutPercent). The Id is left at
    /// its default so the server assigns it.
    /// </summary>
    private Flag AnonymousFlag(string key, int rolloutPercent)
    {
        return _fixture.Build<Flag>()
            .With(f => f.Key, key)
            .With(f => f.RolloutPercent, rolloutPercent)
            .Without(f => f.Id)
            .Create();
    }
}
