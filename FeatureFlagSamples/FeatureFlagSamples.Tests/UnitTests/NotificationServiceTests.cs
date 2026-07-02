using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using FeatureFlagSamples.Services;

namespace FeatureFlagSamples.Tests.UnitTests;

/// <summary>
/// Tests for notification service implementations.
/// Verifies behavior of different service variants.
/// </summary>
public class NotificationServiceTests
{
    [Fact]
    public async Task BasicNotificationService_ShouldSendNotification()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<BasicNotificationService>>();
        var service = new BasicNotificationService(mockLogger.Object);
        var message = "Test notification";

        // Act
        await service.SendNotificationAsync(message);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Basic") && v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task AdvancedNotificationService_ShouldSendNotificationWithEnhancements()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<AdvancedNotificationService>>();
        var service = new AdvancedNotificationService(mockLogger.Object);
        var message = "Test advanced notification";

        // Act
        await service.SendNotificationAsync(message);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Advanced") && v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Short")]
    [InlineData("This is a longer notification message with more content")]
    public async Task NotificationServices_ShouldHandleVariousMessageLengths(string message)
    {
        // Arrange
        var basicLogger = new Mock<ILogger<BasicNotificationService>>();
        var advancedLogger = new Mock<ILogger<AdvancedNotificationService>>();

        var basicService = new BasicNotificationService(basicLogger.Object);
        var advancedService = new AdvancedNotificationService(advancedLogger.Object);

        // Act
        var basicTask = basicService.SendNotificationAsync(message);
        var advancedTask = advancedService.SendNotificationAsync(message);

        // Assert
        await basicTask;
        await advancedTask;

        basicTask.IsCompletedSuccessfully.Should().BeTrue();
        advancedTask.IsCompletedSuccessfully.Should().BeTrue();
    }
}
