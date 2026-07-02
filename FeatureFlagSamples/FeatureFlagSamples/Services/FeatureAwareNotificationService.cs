using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;

namespace FeatureFlagSamples.Services;

/// <summary>
/// Notification service that adapts its behavior based on feature flags.
/// Demonstrates runtime feature flag evaluation within a service.
/// </summary>
public class FeatureAwareNotificationService : INotificationService
{
    private readonly IFeatureManager _featureManager;
    private readonly ILogger<FeatureAwareNotificationService> _logger;

    public FeatureAwareNotificationService(
        IFeatureManager featureManager,
        ILogger<FeatureAwareNotificationService> logger)
    {
        _featureManager = featureManager;
        _logger = logger;
    }

    public async Task SendNotificationAsync(string message)
    {
        var useAdvancedLogging = await _featureManager.IsEnabledAsync(FeatureFlags.AdvancedLogging);

        if (useAdvancedLogging)
        {
            _logger.LogInformation("📊 [Feature-Aware] Sending notification with advanced logging: {Message}", message);
            Console.WriteLine($"📊 Feature-Aware Notification: {message}");
            Console.WriteLine($"   ✓ Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"   ✓ Message Length: {message.Length} characters");
        }
        else
        {
            _logger.LogInformation("Sending notification: {Message}", message);
            Console.WriteLine($"Notification: {message}");
        }

        await Task.CompletedTask;
    }
}
