using FluentAssertions;
using FeatureFlagSamples.Features;

namespace FeatureFlagSamples.Tests.UnitTests;

/// <summary>
/// Tests to verify that all feature flag constants are properly defined.
/// Ensures consistency and prevents duplicate or null values.
/// </summary>
public class FeatureFlagConstantsTests
{
    [Fact]
    public void FeatureFlags_ShouldHaveUniqueValues()
    {
        // Arrange
        var flagValues = new[]
        {
            FeatureFlags.NewUI,
            FeatureFlags.BetaFeatures,
            FeatureFlags.AdvancedLogging,
            FeatureFlags.DatabaseMigration,
            FeatureFlags.PremiumFeatures
        };

        // Act
        var uniqueValues = flagValues.Distinct().ToList();

        // Assert
        uniqueValues.Should().HaveCount(flagValues.Length, "all feature flags should have unique values");
    }

    [Theory]
    [InlineData(nameof(FeatureFlags.NewUI))]
    [InlineData(nameof(FeatureFlags.BetaFeatures))]
    [InlineData(nameof(FeatureFlags.AdvancedLogging))]
    [InlineData(nameof(FeatureFlags.DatabaseMigration))]
    [InlineData(nameof(FeatureFlags.PremiumFeatures))]
    public void FeatureFlags_ShouldNotBeNullOrEmpty(string flagName)
    {
        // Arrange & Act
        var flagValue = typeof(FeatureFlags)
            .GetField(flagName)?
            .GetValue(null) as string;

        // Assert
        flagValue.Should().NotBeNullOrWhiteSpace($"{flagName} should have a non-empty value");
    }

    [Fact]
    public void FeatureFlags_ShouldMatchFieldNames()
    {
        // This ensures the constant value matches its field name for consistency
        // For example: public const string NewUI = "NewUI";

        // Arrange & Act & Assert
        FeatureFlags.NewUI.Should().Be("NewUI");
        FeatureFlags.BetaFeatures.Should().Be("BetaFeatures");
        FeatureFlags.AdvancedLogging.Should().Be("AdvancedLogging");
        FeatureFlags.DatabaseMigration.Should().Be("DatabaseMigration");
        FeatureFlags.PremiumFeatures.Should().Be("PremiumFeatures");
    }

    [Fact]
    public void FeatureFlags_AllFieldsShouldBePublicConst()
    {
        // Arrange
        var type = typeof(FeatureFlags);
        var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        // Act & Assert
        foreach (var field in fields)
        {
            field.IsLiteral.Should().BeTrue($"{field.Name} should be a const");
            field.IsPublic.Should().BeTrue($"{field.Name} should be public");
            field.FieldType.Should().Be(typeof(string), $"{field.Name} should be a string");
        }
    }
}
