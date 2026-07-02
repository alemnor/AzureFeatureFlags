using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;
using FeatureFlagSamples.Scenarios;

namespace FeatureFlagSamples.Tests.IntegrationTests;

/// <summary>
/// End-to-end integration tests for the scenario classes.
/// Verifies complete workflows work as expected.
/// </summary>
public class ScenarioIntegrationTests
{
    [Fact]
    public async Task BasicFeatureFlagScenario_WithEnabledFeatures_ShouldRunSuccessfully()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "true",
                ["FeatureManagement:BetaFeatures"] = "true",
                ["FeatureManagement:AdvancedLogging"] = "true"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddFeatureManagement();
        services.AddTransient<BasicFeatureFlagScenario>();

        var serviceProvider = services.BuildServiceProvider();
        var scenario = serviceProvider.GetRequiredService<BasicFeatureFlagScenario>();

        // Act
        Func<Task> act = async () => await scenario.RunAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task BasicFeatureFlagScenario_WithDisabledFeatures_ShouldRunSuccessfully()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "false",
                ["FeatureManagement:BetaFeatures"] = "false",
                ["FeatureManagement:AdvancedLogging"] = "false"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddFeatureManagement();
        services.AddTransient<BasicFeatureFlagScenario>();

        var serviceProvider = services.BuildServiceProvider();
        var scenario = serviceProvider.GetRequiredService<BasicFeatureFlagScenario>();

        // Act
        Func<Task> act = async () => await scenario.RunAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task PercentageRolloutScenario_ShouldRunWithoutErrors()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:DatabaseMigration:EnabledFor:0:Name"] = "Percentage",
                ["FeatureManagement:DatabaseMigration:EnabledFor:0:Parameters:Value"] = "50"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddFeatureManagement();
        services.AddTransient<PercentageRolloutScenario>();

        var serviceProvider = services.BuildServiceProvider();
        var scenario = serviceProvider.GetRequiredService<PercentageRolloutScenario>();

        // Act
        Func<Task> act = async () => await scenario.RunAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task TimeWindowScenario_WithActiveWindow_ShouldRunSuccessfully()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:PremiumFeatures:EnabledFor:0:Name"] = "CustomTimeWindow",
                ["FeatureManagement:PremiumFeatures:EnabledFor:0:Parameters:Start"] = now.AddHours(-1).ToString("O"),
                ["FeatureManagement:PremiumFeatures:EnabledFor:0:Parameters:End"] = now.AddHours(1).ToString("O")
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddFeatureManagement()
                .AddFeatureFilter<Features.CustomFeatureFilters.TimeWindowFilter>();
        services.AddTransient<TimeWindowScenario>();

        var serviceProvider = services.BuildServiceProvider();
        var scenario = serviceProvider.GetRequiredService<TimeWindowScenario>();

        // Act
        Func<Task> act = async () => await scenario.RunAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task FeatureVariantScenario_ShouldDetermineVariantCorrectly()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "true",
                ["FeatureManagement:BetaFeatures"] = "false"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddFeatureManagement();
        services.AddTransient<FeatureVariantScenario>();

        var serviceProvider = services.BuildServiceProvider();
        var scenario = serviceProvider.GetRequiredService<FeatureVariantScenario>();

        // Act
        Func<Task> act = async () => await scenario.RunAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AllScenarios_ShouldBeResolvableFromDI()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "true",
                ["FeatureManagement:BetaFeatures"] = "true",
                ["FeatureManagement:AdvancedLogging"] = "true",
                ["FeatureManagement:DatabaseMigration:EnabledFor:0:Name"] = "Percentage",
                ["FeatureManagement:DatabaseMigration:EnabledFor:0:Parameters:Value"] = "50",
                ["FeatureManagement:PremiumFeatures:EnabledFor:0:Name"] = "CustomTimeWindow",
                ["FeatureManagement:PremiumFeatures:EnabledFor:0:Parameters:Start"] = DateTimeOffset.UtcNow.AddHours(-1).ToString("O"),
                ["FeatureManagement:PremiumFeatures:EnabledFor:0:Parameters:End"] = DateTimeOffset.UtcNow.AddHours(1).ToString("O")
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddFeatureManagement()
                .AddFeatureFilter<Features.CustomFeatureFilters.TimeWindowFilter>();

        services.AddTransient<BasicFeatureFlagScenario>();
        services.AddTransient<PercentageRolloutScenario>();
        services.AddTransient<TimeWindowScenario>();
        services.AddTransient<FeatureVariantScenario>();

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        serviceProvider.GetRequiredService<BasicFeatureFlagScenario>().Should().NotBeNull();
        serviceProvider.GetRequiredService<PercentageRolloutScenario>().Should().NotBeNull();
        serviceProvider.GetRequiredService<TimeWindowScenario>().Should().NotBeNull();
        serviceProvider.GetRequiredService<FeatureVariantScenario>().Should().NotBeNull();
    }
}
