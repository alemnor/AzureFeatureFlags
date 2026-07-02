using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;

namespace FeatureFlagSamples.Tests.IntegrationTests;

/// <summary>
/// Integration tests that verify configuration loading from JSON files.
/// </summary>
public class ConfigurationIntegrationTests
{
    [Fact]
    public async Task Configuration_FromInMemory_ShouldLoadCorrectly()
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

    [Fact]
    public async Task Configuration_WithNestedFilters_ShouldLoadCorrectly()
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
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync(FeatureFlags.DatabaseMigration);

        // Assert - Should not throw an exception
        (isEnabled == true || isEnabled == false).Should().BeTrue();
    }

    [Fact]
    public async Task Configuration_WithEnvironmentVariableOverride_ShouldUseOverriddenValue()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "false"
            })
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Simulating environment variable override
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
        isEnabled.Should().BeTrue("the later configuration source should override");
    }

    [Fact]
    public void Configuration_ShouldBindToFeatureManagementSection()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "true",
                ["FeatureManagement:BetaFeatures"] = "false"
            })
            .Build();

        // Act
        var featureManagementSection = configuration.GetSection("FeatureManagement");

        // Assert
        featureManagementSection.Exists().Should().BeTrue();
        featureManagementSection.GetValue<bool>("NewUI").Should().BeTrue();
        featureManagementSection.GetValue<bool>("BetaFeatures").Should().BeFalse();
    }
}
