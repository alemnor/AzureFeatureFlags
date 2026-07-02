using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;

namespace FeatureFlagSamples.Scenarios;

/// <summary>
/// Demonstrates time-based feature activation using a custom feature filter.
/// Useful for scheduled releases, promotional periods, or maintenance windows.
/// </summary>
public class TimeWindowScenario
{
    private readonly IFeatureManager _featureManager;
    private readonly ILogger<TimeWindowScenario> _logger;

    public TimeWindowScenario(
        IFeatureManager featureManager,
        ILogger<TimeWindowScenario> logger)
    {
        _featureManager = featureManager;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   Time Window Scenario                   ║");
        Console.WriteLine("╚══════════════════════════════════════════╝\n");

        var currentTime = DateTimeOffset.UtcNow;
        _logger.LogInformation("Checking time-based feature at {CurrentTime}", currentTime);

        Console.WriteLine($"Current UTC Time: {currentTime:yyyy-MM-dd HH:mm:ss}\n");

        var isPremiumEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.PremiumFeatures);

        if (isPremiumEnabled)
        {
            Console.WriteLine("✓ Premium Features ARE AVAILABLE");
            Console.WriteLine("\nAccess granted to:");
            Console.WriteLine("  • Advanced Analytics Dashboard");
            Console.WriteLine("  • Priority Customer Support");
            Console.WriteLine("  • Export to Multiple Formats");
            Console.WriteLine("  • Custom Branding Options");
        }
        else
        {
            Console.WriteLine("○ Premium Features are NOT AVAILABLE");
            Console.WriteLine("\nThis feature is only available during the promotional period.");
            Console.WriteLine("Check the configuration for the active time window.");
        }
    }
}
