using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;

namespace FeatureFlagSamples.Scenarios;

/// <summary>
/// Demonstrates feature variants for A/B/C testing scenarios.
/// Shows how to use feature flags for more than just on/off states.
/// </summary>
public class FeatureVariantScenario
{
    private readonly IFeatureManager _featureManager;
    private readonly ILogger<FeatureVariantScenario> _logger;

    public FeatureVariantScenario(
        IFeatureManager featureManager,
        ILogger<FeatureVariantScenario> logger)
    {
        _featureManager = featureManager;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   Feature Variant Scenario               ║");
        Console.WriteLine("╚══════════════════════════════════════════╝\n");

        _logger.LogInformation("Demonstrating feature variants for A/B testing");

        // Simulate different UI variants based on feature flags
        var variant = await DetermineUIVariantAsync();

        Console.WriteLine($"Active UI Variant: {variant}\n");

        switch (variant)
        {
            case "VariantA":
                Console.WriteLine("🅰️  Displaying UI Variant A (Control)");
                Console.WriteLine("  - Standard button layout");
                Console.WriteLine("  - Blue color scheme");
                Console.WriteLine("  - Sidebar navigation");
                break;

            case "VariantB":
                Console.WriteLine("🅱️  Displaying UI Variant B (Test 1)");
                Console.WriteLine("  - Floating action buttons");
                Console.WriteLine("  - Green color scheme");
                Console.WriteLine("  - Top navigation bar");
                break;

            case "VariantC":
                Console.WriteLine("🅲  Displaying UI Variant C (Test 2)");
                Console.WriteLine("  - Minimalist button design");
                Console.WriteLine("  - Purple color scheme");
                Console.WriteLine("  - Hamburger menu navigation");
                break;
        }
    }

    private async Task<string> DetermineUIVariantAsync()
    {
        // This is a simplified variant selection
        // In production, you might use targeting rules, user segments, etc.

        if (await _featureManager.IsEnabledAsync(FeatureFlags.BetaFeatures))
        {
            return "VariantC";
        }

        if (await _featureManager.IsEnabledAsync(FeatureFlags.NewUI))
        {
            return "VariantB";
        }

        return "VariantA";
    }
}
