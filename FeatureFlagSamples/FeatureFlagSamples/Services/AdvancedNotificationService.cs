using Microsoft.Extensions.Logging;

namespace FeatureFlagSamples.Services;

/// <summary>
/// Advanced implementation of notification service with enhanced features.
/// Used when beta features are enabled.
/// </summary>
public class AdvancedNotificationService : INotificationService
{
    private readonly ILogger<AdvancedNotificationService> _logger;

    public AdvancedNotificationService(ILogger<AdvancedNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendNotificationAsync(string message)
    {
        _logger.LogInformation("🚀 [Advanced] Sending enhanced notification: {Message}", message);
        Console.WriteLine($"🚀 Advanced Notification with tracking: {message}");
        Console.WriteLine("   ✓ Delivery confirmation enabled");
        Console.WriteLine("   ✓ Analytics tracking enabled");
        Console.WriteLine("   ✓ Priority routing enabled");
        return Task.CompletedTask;
    }
}
