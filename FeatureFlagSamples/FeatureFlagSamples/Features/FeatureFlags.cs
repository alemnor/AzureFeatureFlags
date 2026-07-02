namespace FeatureFlagSamples.Features;

/// <summary>
/// Central repository of all feature flag names used in the application.
/// Using constants ensures compile-time checking and prevents typos.
/// </summary>
public static class FeatureFlags
{
    public const string NewUI = "NewUI";
    public const string BetaFeatures = "BetaFeatures";
    public const string AdvancedLogging = "AdvancedLogging";
    public const string DatabaseMigration = "DatabaseMigration";
    public const string PremiumFeatures = "PremiumFeatures";
}
