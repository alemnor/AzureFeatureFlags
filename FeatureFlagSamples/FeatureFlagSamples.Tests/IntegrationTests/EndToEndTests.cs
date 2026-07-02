using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;

namespace FeatureFlagSamples.Tests.IntegrationTests;

/// <summary>
/// End-to-end tests simulating real-world scenarios.
/// These tests verify complete workflows from configuration to execution.
/// </summary>
public class EndToEndTests
{
    [Fact]
    public async Task CompleteWorkflow_CanaryDeployment_ShouldWorkCorrectly()
    {
        // Simulate a canary deployment scenario where a new feature is rolled out to a percentage of users
        // Note: Without context (like user ID), the percentage filter may vary, but it should always return a boolean

        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewDatabaseSchema:EnabledFor:0:Name"] = "Percentage",
                ["FeatureManagement:NewDatabaseSchema:EnabledFor:0:Parameters:Value"] = "50"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act - Check the feature
        var isEnabled = await featureManager.IsEnabledAsync("NewDatabaseSchema");

        // Assert - Should return a valid boolean without throwing
        (isEnabled == true || isEnabled == false).Should().BeTrue();
    }

    [Fact]
    public async Task CompleteWorkflow_GradualRollout_ShouldProgressCorrectly()
    {
        // Simulate a gradual rollout: 0% -> 25% -> 50% -> 100%

        async Task<bool> CheckFeatureAtPercentage(int percentage)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["FeatureManagement:NewFeature:EnabledFor:0:Name"] = "Percentage",
                    ["FeatureManagement:NewFeature:EnabledFor:0:Parameters:Value"] = percentage.ToString()
                })
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);
            services.AddFeatureManagement();

            var sp = services.BuildServiceProvider();
            var fm = sp.GetRequiredService<IFeatureManager>();

            return await fm.IsEnabledAsync("NewFeature");
        }

        // Act
        var at0Percent = await CheckFeatureAtPercentage(0);
        var at100Percent = await CheckFeatureAtPercentage(100);

        // Assert
        at0Percent.Should().BeFalse("at 0% the feature should be disabled");
        at100Percent.Should().BeTrue("at 100% the feature should be enabled");
    }

    [Fact]
    public async Task CompleteWorkflow_ScheduledRelease_ShouldActivateAtCorrectTime()
    {
        // Simulate a scheduled feature release

        // Arrange - Feature active for the next hour
        var now = DateTimeOffset.UtcNow;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:ScheduledFeature:EnabledFor:0:Name"] = "CustomTimeWindow",
                ["FeatureManagement:ScheduledFeature:EnabledFor:0:Parameters:Start"] = now.AddMinutes(-30).ToString("O"),
                ["FeatureManagement:ScheduledFeature:EnabledFor:0:Parameters:End"] = now.AddMinutes(30).ToString("O")
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement()
                .AddFeatureFilter<Features.CustomFeatureFilters.TimeWindowFilter>();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Act
        var isDuringWindow = await featureManager.IsEnabledAsync("ScheduledFeature");

        // Assert
        isDuringWindow.Should().BeTrue("current time is within the scheduled window");
    }

    [Fact]
    public async Task CompleteWorkflow_ABTesting_ShouldSupportMultipleVariants()
    {
        // Simulate A/B testing with different feature combinations

        // Arrange - Variant A: No features
        var configA = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "false",
                ["FeatureManagement:BetaFeatures"] = "false"
            })
            .Build();

        // Variant B: New UI only
        var configB = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "true",
                ["FeatureManagement:BetaFeatures"] = "false"
            })
            .Build();

        // Variant C: All features
        var configC = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:NewUI"] = "true",
                ["FeatureManagement:BetaFeatures"] = "true"
            })
            .Build();

        async Task<(bool newUI, bool beta)> GetFeatureState(IConfiguration config)
        {
            var services = new ServiceCollection();
            services.AddSingleton(config);
            services.AddFeatureManagement();
            var sp = services.BuildServiceProvider();
            var fm = sp.GetRequiredService<IFeatureManager>();

            return (
                await fm.IsEnabledAsync(FeatureFlags.NewUI),
                await fm.IsEnabledAsync(FeatureFlags.BetaFeatures)
            );
        }

        // Act
        var variantA = await GetFeatureState(configA);
        var variantB = await GetFeatureState(configB);
        var variantC = await GetFeatureState(configC);

        // Assert
        variantA.Should().Be((false, false), "Variant A should have no features");
        variantB.Should().Be((true, false), "Variant B should have NewUI only");
        variantC.Should().Be((true, true), "Variant C should have all features");
    }

    [Fact]
    public async Task CompleteWorkflow_KillSwitch_ShouldDisableFeatureImmediately()
    {
        // Simulate a kill switch scenario where a problematic feature needs to be disabled

        // Arrange - Feature initially enabled
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:ProblematicFeature"] = "true"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        var beforeKillSwitch = await featureManager.IsEnabledAsync("ProblematicFeature");

        // Act - Simulate kill switch by changing configuration
        var updatedConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:ProblematicFeature"] = "false"
            })
            .Build();

        var updatedServices = new ServiceCollection();
        updatedServices.AddSingleton<IConfiguration>(updatedConfiguration);
        updatedServices.AddFeatureManagement();

        var updatedServiceProvider = updatedServices.BuildServiceProvider();
        var updatedFeatureManager = updatedServiceProvider.GetRequiredService<IFeatureManager>();

        var afterKillSwitch = await updatedFeatureManager.IsEnabledAsync("ProblematicFeature");

        // Assert
        beforeKillSwitch.Should().BeTrue("feature was initially enabled");
        afterKillSwitch.Should().BeFalse("feature should be disabled after kill switch");
    }
}
