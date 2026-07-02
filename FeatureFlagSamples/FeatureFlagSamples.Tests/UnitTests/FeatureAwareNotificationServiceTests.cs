using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Moq;
using FeatureFlagSamples.Features;
using FeatureFlagSamples.Services;

namespace FeatureFlagSamples.Tests.UnitTests;

/// <summary>
/// Tests for the FeatureAwareNotificationService which adapts behavior based on feature flags.
/// </summary>
public class FeatureAwareNotificationServiceTests
{
    [Fact]
    public async Task SendNotification_WhenAdvancedLoggingEnabled_ShouldUseAdvancedFormat()
    {
        // Arrange
        var mockFeatureManager = new Mock<IFeatureManager>();
        var mockLogger = new Mock<ILogger<FeatureAwareNotificationService>>();

        mockFeatureManager
            .Setup(x => x.IsEnabledAsync(FeatureFlags.AdvancedLogging))
            .ReturnsAsync(true);

        var service = new FeatureAwareNotificationService(mockFeatureManager.Object, mockLogger.Object);
        var message = "Test message";

        // Act
        await service.SendNotificationAsync(message);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Feature-Aware") && v.ToString()!.Contains("advanced logging")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        mockFeatureManager.Verify(x => x.IsEnabledAsync(FeatureFlags.AdvancedLogging), Times.Once);
    }

    [Fact]
    public async Task SendNotification_WhenAdvancedLoggingDisabled_ShouldUseBasicFormat()
    {
        // Arrange
        var mockFeatureManager = new Mock<IFeatureManager>();
        var mockLogger = new Mock<ILogger<FeatureAwareNotificationService>>();

        mockFeatureManager
            .Setup(x => x.IsEnabledAsync(FeatureFlags.AdvancedLogging))
            .ReturnsAsync(false);

        var service = new FeatureAwareNotificationService(mockFeatureManager.Object, mockLogger.Object);
        var message = "Test message";

        // Act
        await service.SendNotificationAsync(message);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message) && !v.ToString()!.Contains("advanced logging")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        mockFeatureManager.Verify(x => x.IsEnabledAsync(FeatureFlags.AdvancedLogging), Times.Once);
    }

    [Fact]
    public async Task SendNotification_ShouldCheckFeatureFlagOnEveryCall()
    {
        // Arrange
        var mockFeatureManager = new Mock<IFeatureManager>();
        var mockLogger = new Mock<ILogger<FeatureAwareNotificationService>>();

        mockFeatureManager
            .Setup(x => x.IsEnabledAsync(FeatureFlags.AdvancedLogging))
            .ReturnsAsync(true);

        var service = new FeatureAwareNotificationService(mockFeatureManager.Object, mockLogger.Object);

        // Act
        await service.SendNotificationAsync("Message 1");
        await service.SendNotificationAsync("Message 2");
        await service.SendNotificationAsync("Message 3");

        // Assert - Feature flag should be checked for each call
        mockFeatureManager.Verify(x => x.IsEnabledAsync(FeatureFlags.AdvancedLogging), Times.Exactly(3));
    }
}
