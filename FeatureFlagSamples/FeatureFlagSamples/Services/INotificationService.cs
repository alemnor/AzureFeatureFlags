namespace FeatureFlagSamples.Services;

/// <summary>
/// Example service interface to demonstrate feature flag usage in service implementations.
/// </summary>
public interface INotificationService
{
    Task SendNotificationAsync(string message);
}
