using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureFlagSamples.Tests.UnitTests;

/// <summary>
/// Tests for the custom TimeWindowFilter feature filter.
/// Verifies time-based feature activation logic.
/// </summary>
public class TimeWindowFilterTests
{
    [Fact]
    public async Task TimeWindowFilter_WhenCurrentTimeIsWithinWindow_ShouldReturnTrue()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var start = now.AddHours(-1);
        var end = now.AddHours(1);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:TestFeature:EnabledFor:0:Name"] = "CustomTimeWindow",
                ["FeatureManagement:TestFeature:EnabledFor:0:Parameters:Start"] = start.ToString("O"),
                ["FeatureManagement:TestFeature:EnabledFor:0:Parameters:End"] = end.ToString("O")
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement()
                .AddFeatureFilter<Features.CustomFeatureFilters.TimeWindowFilter>();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync("TestFeature");

        // Assert
        isEnabled.Should().BeTrue("current time is within the configured time window");
    }

    [Fact]
    public async Task TimeWindowFilter_WhenCurrentTimeIsBeforeWindow_ShouldReturnFalse()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var start = now.AddHours(1);
        var end = now.AddHours(2);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:TestFeature:EnabledFor:0:Name"] = "CustomTimeWindow",
                ["FeatureManagement:TestFeature:EnabledFor:0:Parameters:Start"] = start.ToString("O"),
                ["FeatureManagement:TestFeature:EnabledFor:0:Parameters:End"] = end.ToString("O")
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement()
                .AddFeatureFilter<Features.CustomFeatureFilters.TimeWindowFilter>();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync("TestFeature");

        // Assert
        isEnabled.Should().BeFalse("current time is before the configured time window");
    }

    [Fact]
    public async Task TimeWindowFilter_WhenCurrentTimeIsAfterWindow_ShouldReturnFalse()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var start = now.AddHours(-2);
        var end = now.AddHours(-1);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:TestFeature:EnabledFor:0:Name"] = "CustomTimeWindow",
                ["FeatureManagement:TestFeature:EnabledFor:0:Parameters:Start"] = start.ToString("O"),
                ["FeatureManagement:TestFeature:EnabledFor:0:Parameters:End"] = end.ToString("O")
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement()
                .AddFeatureFilter<Features.CustomFeatureFilters.TimeWindowFilter>();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync("TestFeature");

        // Assert
        isEnabled.Should().BeFalse("current time is after the configured time window");
    }

    [Fact]
    public async Task TimeWindowFilter_WhenParametersAreMissing_ShouldReturnFalse()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:TestFeature:EnabledFor:0:Name"] = "CustomTimeWindow"
                // No Start/End parameters
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement()
                .AddFeatureFilter<Features.CustomFeatureFilters.TimeWindowFilter>();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isEnabled = await featureManager.IsEnabledAsync("TestFeature");

        // Assert
        isEnabled.Should().BeFalse("parameters are missing");
    }
}
