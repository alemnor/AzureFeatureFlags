using Microsoft.Extensions.Logging;

namespace FeatureFlagSamples.Services;

/// <summary>
/// Basic implementation of notification service.
/// Used when feature flags are disabled.
/// </summary>
public class BasicNotificationService : INotificationService
{
    private readonly ILogger<BasicNotificationService> _logger;

    public BasicNotificationService(ILogger<BasicNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendNotificationAsync(string message)
    {
        _logger.LogInformation("📧 [Basic] Sending notification: {Message}", message);
        Console.WriteLine($"📧 Basic Notification: {message}");
        return Task.CompletedTask;
    }
}
