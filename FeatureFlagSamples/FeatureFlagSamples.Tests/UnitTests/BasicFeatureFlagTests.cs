using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;

namespace FeatureFlagSamples.Tests.UnitTests;

/// <summary>
/// Tests for basic feature flag functionality.
/// Verifies simple on/off toggle behavior.
/// </summary>
public class BasicFeatureFlagTests
{
    [Fact]
    public async Task FeatureFlag_WhenEnabled_ShouldReturnTrue()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "true"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync(FeatureFlags.NewUI);

        // Assert
        isEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task FeatureFlag_WhenDisabled_ShouldReturnFalse()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "false"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync(FeatureFlags.NewUI);

        // Assert
        isEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task FeatureFlag_WhenNotConfigured_ShouldReturnFalse()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync(FeatureFlags.NewUI);

        // Assert
        isEnabled.Should().BeFalse("unconfigured features default to disabled");
    }

    [Fact]
    public async Task MultipleFeatureFlags_ShouldBeEvaluatedIndependently()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "true",
                ["FeatureManagement:BetaFeatures"] = "false",
                ["FeatureManagement:AdvancedLogging"] = "true"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var newUIEnabled = await featureManager.IsEnabledAsync(FeatureFlags.NewUI);
        var betaEnabled = await featureManager.IsEnabledAsync(FeatureFlags.BetaFeatures);
        var loggingEnabled = await featureManager.IsEnabledAsync(FeatureFlags.AdvancedLogging);

        // Assert
        newUIEnabled.Should().BeTrue();
        betaEnabled.Should().BeFalse();
        loggingEnabled.Should().BeTrue();
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("True", true)]
    [InlineData("TRUE", true)]
    [InlineData("false", false)]
    [InlineData("False", false)]
    [InlineData("FALSE", false)]
    public async Task FeatureFlag_ShouldBeCaseInsensitive(string configValue, bool expected)
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = configValue
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync(FeatureFlags.NewUI);

        // Assert
        isEnabled.Should().Be(expected);
    }
}
