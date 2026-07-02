using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FeatureFlagSamples.Extensions;
using FeatureFlagSamples.Scenarios;
using FeatureFlagSamples.Features;
using Microsoft.FeatureManagement;

// Build host with configuration and dependency injection
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        var basePath = AppContext.BaseDirectory;
        config.SetBasePath(basePath)
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", 
                          optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        // Register feature management with filters
        services.AddFeatureFlagServices();

        // Build an intermediate service provider to access IFeatureManager
        var serviceProvider = services.BuildServiceProvider();
        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

        // Register conditional services based on feature flags
        services.AddConditionalServices(featureManager);

        // Register scenario classes
        services.AddTransient<BasicFeatureFlagScenario>();
        services.AddTransient<ConditionalDependencyScenario>();
        services.AddTransient<PercentageRolloutScenario>();
        services.AddTransient<TimeWindowScenario>();
        services.AddTransient<FeatureVariantScenario>();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    })
    .Build();

// Display welcome banner
Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                                                            ║");
Console.WriteLine("║        Feature Flag Samples for .NET                       ║");
Console.WriteLine("║        Demonstrating Best Practices                        ║");
Console.WriteLine("║                                                            ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var featureManager = host.Services.GetRequiredService<IFeatureManager>();

try
{
    // Display current environment
    var environment = host.Services.GetRequiredService<IHostEnvironment>();
    Console.WriteLine($"Environment: {environment.EnvironmentName}");
    Console.WriteLine($"Configuration loaded from appsettings.{environment.EnvironmentName}.json\n");

    // Display all feature flag states
    await DisplayFeatureFlagStatusAsync(featureManager, logger);

    // Run demonstration scenarios
    Console.WriteLine("\n" + new string('═', 60));
    Console.WriteLine("Starting Feature Flag Demonstration Scenarios");
    Console.WriteLine(new string('═', 60));

    // 1. Basic Feature Flags
    var basicScenario = host.Services.GetRequiredService<BasicFeatureFlagScenario>();
    await basicScenario.RunAsync();

    // 2. Conditional Dependency Injection
    var dependencyScenario = host.Services.GetRequiredService<ConditionalDependencyScenario>();
    await dependencyScenario.RunAsync();

    // 3. Percentage Rollout
    var rolloutScenario = host.Services.GetRequiredService<PercentageRolloutScenario>();
    await rolloutScenario.RunAsync();

    // 4. Time Window
    var timeWindowScenario = host.Services.GetRequiredService<TimeWindowScenario>();
    await timeWindowScenario.RunAsync();

    // 5. Feature Variants (A/B Testing)
    var variantScenario = host.Services.GetRequiredService<FeatureVariantScenario>();
    await variantScenario.RunAsync();

    // Completion message
    Console.WriteLine("\n" + new string('═', 60));
    Console.WriteLine("✓ All scenarios completed successfully!");
    Console.WriteLine(new string('═', 60));
    Console.WriteLine("\n💡 Tips:");
    Console.WriteLine("   • Modify appsettings.json to change feature flags");
    Console.WriteLine("   • Set ASPNETCORE_ENVIRONMENT=Development for dev config");
    Console.WriteLine("   • Review README.md for detailed explanations");
    Console.WriteLine("   • Check the code comments for implementation details\n");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while running the feature flag samples");
    Console.WriteLine($"\n❌ Error: {ex.Message}");
    return 1;
}

return 0;

// Helper method to display all feature flag states
static async Task DisplayFeatureFlagStatusAsync(IFeatureManager featureManager, ILogger logger)
{
    Console.WriteLine("Current Feature Flag Status:");
    Console.WriteLine(new string('─', 60));

    var featureNames = new[]
    {
        FeatureFlags.NewUI,
        FeatureFlags.BetaFeatures,
        FeatureFlags.AdvancedLogging,
        FeatureFlags.DatabaseMigration,
        FeatureFlags.PremiumFeatures
    };

    foreach (var featureName in featureNames)
    {
        var isEnabled = await featureManager.IsEnabledAsync(featureName);
        var status = isEnabled ? "✓ Enabled " : "○ Disabled";
        var color = isEnabled ? "🟢" : "⚫";

        Console.WriteLine($"  {color} {featureName,-25} {status}");
        logger.LogDebug("Feature {FeatureName} is {Status}", featureName, isEnabled ? "enabled" : "disabled");
    }

    Console.WriteLine(new string('─', 60));
}
