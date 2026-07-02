using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;
using FeatureFlagSamples.Features.CustomFeatureFilters;
using FeatureFlagSamples.Services;

namespace FeatureFlagSamples.Extensions;

/// <summary>
/// Extension methods for configuring services with feature flag support.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFeatureFlagServices(this IServiceCollection services)
    {
        // Register feature management with custom filters
        services.AddFeatureManagement()
                .AddFeatureFilter<TimeWindowFilter>();

        return services;
    }

    public static IServiceCollection AddConditionalServices(
        this IServiceCollection services,
        IFeatureManager featureManager)
    {
        // Determine which notification service to register based on feature flags
        // Note: This is evaluated at startup. For runtime switching, use IFeatureManager in the service.

        var useBetaFeatures = featureManager.IsEnabledAsync(FeatureFlags.BetaFeatures)
            .GetAwaiter()
            .GetResult();

        if (useBetaFeatures)
        {
            services.AddSingleton<INotificationService, AdvancedNotificationService>();
        }
        else
        {
            services.AddSingleton<INotificationService, BasicNotificationService>();
        }

        // Also register the feature-aware service for demonstration
        services.AddSingleton<FeatureAwareNotificationService>();

        return services;
    }
}
