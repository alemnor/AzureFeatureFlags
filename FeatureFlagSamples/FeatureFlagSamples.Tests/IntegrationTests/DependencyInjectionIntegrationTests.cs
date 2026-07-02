using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;
using FeatureFlagSamples.Services;

namespace FeatureFlagSamples.Tests.IntegrationTests;

/// <summary>
/// Integration tests that verify the complete dependency injection setup.
/// </summary>
public class DependencyInjectionIntegrationTests
{
    [Fact]
    public void ServiceCollection_ShouldRegisterFeatureManagement()
    {
        // Arrange & Act
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var featureManager = serviceProvider.GetService<IFeatureManager>();
        featureManager.Should().NotBeNull("IFeatureManager should be registered");
    }

    [Fact]
    public void ServiceCollection_ShouldRegisterCustomFilters()
    {
        // Arrange & Act
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        services.AddFeatureManagement()
                .AddFeatureFilter<Features.CustomFeatureFilters.TimeWindowFilter>();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var featureManager = serviceProvider.GetService<IFeatureManager>();
        featureManager.Should().NotBeNull();

        // The filter should be usable (we won't get an exception about unknown filter)
        // This is verified by the TimeWindowFilterTests
    }

    [Fact]
    public async Task ConditionalServiceRegistration_WhenBetaFeaturesEnabled_ShouldRegisterAdvancedService()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:BetaFeatures"] = "true"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Manually register based on feature flag (simulating the Extensions method)
        var useBetaFeatures = await featureManager.IsEnabledAsync(FeatureFlags.BetaFeatures);

        if (useBetaFeatures)
        {
            services.AddSingleton<INotificationService, AdvancedNotificationService>();
        }
        else
        {
            services.AddSingleton<INotificationService, BasicNotificationService>();
        }

        var finalServiceProvider = services.BuildServiceProvider();

        // Act
        var notificationService = finalServiceProvider.GetRequiredService<INotificationService>();

        // Assert
        notificationService.Should().BeOfType<AdvancedNotificationService>(
            "BetaFeatures is enabled so AdvancedNotificationService should be registered");
    }

    [Fact]
    public async Task ConditionalServiceRegistration_WhenBetaFeaturesDisabled_ShouldRegisterBasicService()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FeatureManagement:BetaFeatures"] = "false"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddFeatureManagement();

        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Manually register based on feature flag (simulating the Extensions method)
        var useBetaFeatures = await featureManager.IsEnabledAsync(FeatureFlags.BetaFeatures);

        if (useBetaFeatures)
        {
            services.AddSingleton<INotificationService, AdvancedNotificationService>();
        }
        else
        {
            services.AddSingleton<INotificationService, BasicNotificationService>();
        }

        var finalServiceProvider = services.BuildServiceProvider();

        // Act
        var notificationService = finalServiceProvider.GetRequiredService<INotificationService>();

        // Assert
        notificationService.Should().BeOfType<BasicNotificationService>(
            "BetaFeatures is disabled so BasicNotificationService should be registered");
    }

    [Fact]
    public void HostBuilder_ShouldConfigureServicesCorrectly()
    {
        // Arrange & Act
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["FeatureManagement:NewUI"] = "true"
                });
            })
            .ConfigureServices((context, services) =>
            {
                services.AddFeatureManagement();
            })
            .Build();

        // Assert
        var featureManager = host.Services.GetService<IFeatureManager>();
        featureManager.Should().NotBeNull();
    }
}
