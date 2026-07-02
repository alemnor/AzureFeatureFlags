using Microsoft.Extensions.Logging;
using FeatureFlagSamples.Services;

namespace FeatureFlagSamples.Scenarios;

/// <summary>
/// Demonstrates how to inject different service implementations based on feature flags.
/// This pattern allows for clean A/B testing and gradual rollouts.
/// </summary>
public class ConditionalDependencyScenario
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<ConditionalDependencyScenario> _logger;

    public ConditionalDependencyScenario(
        INotificationService notificationService,
        ILogger<ConditionalDependencyScenario> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════╗");
        Console.WriteLine("║   Conditional Dependency Scenario        ║");
        Console.WriteLine("╚══════════════════════════════════════════╝\n");

        _logger.LogInformation("Demonstrating feature-based service injection");

        Console.WriteLine("Sending notifications using injected service implementation...\n");

        await _notificationService.SendNotificationAsync("Welcome to the application!");
        Console.WriteLine();

        await _notificationService.SendNotificationAsync("Your order has been processed");
        Console.WriteLine();

        await _notificationService.SendNotificationAsync("New features are available");
    }
}
