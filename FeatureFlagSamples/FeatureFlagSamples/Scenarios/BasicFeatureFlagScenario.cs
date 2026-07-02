using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;

namespace FeatureFlagSamples.Scenarios;

/// <summary>
/// Demonstrates basic feature flag usage with simple boolean checks.
/// </summary>
public class BasicFeatureFlagScenario
{
    private readonly IFeatureManager _featureManager;
    private readonly ILogger<BasicFeatureFlagScenario> _logger;

    public BasicFeatureFlagScenario(
        IFeatureManager featureManager,
        ILogger<BasicFeatureFlagScenario> logger)
    {
        _featureManager = featureManager;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   Basic Feature Flag Scenario            ║");
        Console.WriteLine("╚══════════════════════════════════════════╝\n");

        // Simple boolean check
        if (await _featureManager.IsEnabledAsync(FeatureFlags.NewUI))
        {
            _logger.LogInformation("New UI is enabled - showing modern interface");
            Console.WriteLine("✓ Displaying NEW modern UI dashboard");
            Console.WriteLine("  - Material Design components");
            Console.WriteLine("  - Dark mode support");
            Console.WriteLine("  - Responsive layout");
        }
        else
        {
            _logger.LogInformation("New UI is disabled - showing classic interface");
            Console.WriteLine("○ Displaying CLASSIC UI dashboard");
            Console.WriteLine("  - Traditional layout");
            Console.WriteLine("  - Standard components");
        }

        Console.WriteLine();

        // Check multiple features
        var betaEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.BetaFeatures);
        var advancedLogging = await _featureManager.IsEnabledAsync(FeatureFlags.AdvancedLogging);

        Console.WriteLine("Feature Status Summary:");
        Console.WriteLine($"  Beta Features: {(betaEnabled ? "✓ Enabled" : "○ Disabled")}");
        Console.WriteLine($"  Advanced Logging: {(advancedLogging ? "✓ Enabled" : "○ Disabled")}");
    }
}
