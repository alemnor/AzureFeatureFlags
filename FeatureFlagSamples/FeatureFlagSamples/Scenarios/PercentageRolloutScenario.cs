using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using FeatureFlagSamples.Features;

namespace FeatureFlagSamples.Scenarios;

/// <summary>
/// Demonstrates percentage-based feature rollout.
/// Useful for gradual rollouts and A/B testing.
/// </summary>
public class PercentageRolloutScenario
{
    private readonly IFeatureManager _featureManager;
    private readonly ILogger<PercentageRolloutScenario> _logger;

    public PercentageRolloutScenario(
        IFeatureManager featureManager,
        ILogger<PercentageRolloutScenario> logger)
    {
        _featureManager = featureManager;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   Percentage Rollout Scenario            ║");
        Console.WriteLine("╚══════════════════════════════════════════╝\n");

        _logger.LogInformation("Testing database migration feature with 50% rollout");

        Console.WriteLine("Simulating 10 user requests with 50% feature rollout...\n");

        int enabledCount = 0;
        for (int i = 1; i <= 10; i++)
        {
            // The percentage filter uses a consistent hash, so the same context 
            // will always get the same result
            var isEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.DatabaseMigration);

            if (isEnabled)
            {
                enabledCount++;
                Console.WriteLine($"User {i,2}: ✓ Using NEW database schema");
            }
            else
            {
                Console.WriteLine($"User {i,2}: ○ Using OLD database schema");
            }
        }

        Console.WriteLine($"\nRollout Statistics:");
        Console.WriteLine($"  Enabled: {enabledCount}/10 ({enabledCount * 10}%)");
        Console.WriteLine($"  Target:  50%");
        Console.WriteLine($"\n  Note: Actual percentage may vary due to hashing algorithm");
    }
}
