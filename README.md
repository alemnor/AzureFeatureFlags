# Feature Flag Samples for C#

This solution demonstrates **correct implementation patterns** for feature flags in .NET applications using Microsoft.FeatureManagement.

## 🎯 Purpose

Feature flags (also called feature toggles) allow you to:
- Enable/disable features without redeploying code
- Perform gradual rollouts (canary releases)
- A/B test different implementations
- Manage features across environments
- Implement kill switches for problematic features

## 📁 Solution Structure

```
FeatureFlagSamples/
├── FeatureFlagSamples/                      # Main application
│   ├── Features/
│   │   ├── FeatureFlags.cs                  # Central feature flag constants
│   │   └── CustomFeatureFilters/
│   │       └── TimeWindowFilter.cs          # Custom time-based filter
│   ├── Services/
│   │   ├── INotificationService.cs          # Service interface
│   │   ├── BasicNotificationService.cs      # Basic implementation
│   │   ├── AdvancedNotificationService.cs   # Advanced implementation
│   │   └── FeatureAwareNotificationService.cs # Runtime feature evaluation
│   ├── Scenarios/
│   │   ├── BasicFeatureFlagScenario.cs      # Simple on/off toggles
│   │   ├── ConditionalDependencyScenario.cs # Dependency injection based on flags
│   │   ├── PercentageRolloutScenario.cs     # Gradual rollout patterns
│   │   ├── TimeWindowScenario.cs            # Time-based activation
│   │   └── FeatureVariantScenario.cs        # A/B/C testing patterns
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs   # DI configuration helpers
│   ├── appsettings.json                     # Production feature configuration
│   ├── appsettings.Development.json         # Development overrides
│   └── Program.cs                           # Application entry point
│
└── FeatureFlagSamples.Tests/                # ✨ Comprehensive test suite
    ├── UnitTests/                           # 30+ unit tests
    │   ├── BasicFeatureFlagTests.cs
    │   ├── TimeWindowFilterTests.cs
    │   ├── NotificationServiceTests.cs
    │   ├── FeatureAwareNotificationServiceTests.cs
    │   ├── PercentageRolloutTests.cs
    │   └── FeatureFlagConstantsTests.cs
    ├── IntegrationTests/                    # 26+ integration tests
    │   ├── DependencyInjectionIntegrationTests.cs
    │   ├── ConfigurationIntegrationTests.cs
    │   ├── ScenarioIntegrationTests.cs
    │   └── EndToEndTests.cs
    └── README.md                            # Test documentation
```

## 🔑 Key Patterns Demonstrated

### 1. **Centralized Feature Flag Names**
Use constants instead of magic strings to prevent typos and enable refactoring:

```csharp
public static class FeatureFlags
{
	public const string NewUI = "NewUI";
	public const string BetaFeatures = "BetaFeatures";
}
```

### 2. **Configuration-Based Flags**
Define flags in `appsettings.json` for easy management:

```json
{
  "FeatureManagement": {
	"NewUI": true,
	"BetaFeatures": false
  }
}
```

### 3. **Runtime Feature Evaluation**
Check features at runtime for dynamic behavior:

```csharp
if (await _featureManager.IsEnabledAsync(FeatureFlags.NewUI))
{
	// Show new UI
}
else
{
	// Show old UI
}
```

### 4. **Conditional Service Registration**
Inject different implementations based on feature state:

```csharp
if (await featureManager.IsEnabledAsync(FeatureFlags.BetaFeatures))
{
	services.AddSingleton<INotificationService, AdvancedNotificationService>();
}
else
{
	services.AddSingleton<INotificationService, BasicNotificationService>();
}
```

### 5. **Percentage-Based Rollouts**
Gradually roll out features to a percentage of users:

```json
{
  "FeatureManagement": {
	"DatabaseMigration": {
	  "EnabledFor": [
		{
		  "Name": "Percentage",
		  "Parameters": { "Value": 50 }
		}
	  ]
	}
  }
}
```

### 6. **Custom Feature Filters**
Create custom logic for feature activation:

```csharp
[FilterAlias("TimeWindow")]
public class TimeWindowFilter : IFeatureFilter
{
	public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
	{
		// Custom time-based logic
	}
}
```

## 🚀 Running the Samples

1. **Build the solution:**
   ```bash
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Modify feature flags:**
   - Edit `appsettings.json` or `appsettings.Development.json`
   - Set environment variable `ASPNETCORE_ENVIRONMENT=Development`
   - Run again to see different behavior

## 🎓 Best Practices

✅ **DO:**
- Use centralized feature flag constants
- Document what each feature flag controls
- Remove flags after full rollout
- Use environment-specific configurations
- Log feature flag evaluations for debugging
- Use feature filters for complex scenarios

❌ **DON'T:**
- Use magic strings for feature names
- Leave old feature flags in code indefinitely
- Nest feature flags too deeply
- Use feature flags for configuration values
- Forget to test both enabled and disabled states

## 🧪 Testing

The solution includes a **comprehensive test suite** with 56+ tests:

### Run All Tests
```bash
dotnet test
```

### Test Coverage
- ✅ **Unit Tests** (30+): Basic flags, filters, services
- ✅ **Integration Tests** (26+): DI, configuration, scenarios
- ✅ **100% Pass Rate**: All tests passing
- ✅ **Fast Execution**: < 2 seconds

See `FeatureFlagSamples.Tests/README.md` for detailed test documentation.

### Test Categories
```bash
# Run unit tests only
dotnet test --filter "FullyQualifiedName~UnitTests"

# Run integration tests only
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Run with code coverage
dotnet test /p:CollectCoverage=true
```

## 📚 Learn More

- [Microsoft.FeatureManagement Documentation](https://github.com/microsoft/FeatureManagement-Dotnet)
- [Azure App Configuration Feature Flags](https://learn.microsoft.com/azure/azure-app-configuration/concept-feature-management)
- [Feature Toggle Patterns](https://martinfowler.com/articles/feature-toggles.html)

## 🔧 Extension Points

This sample can be extended with:
- **Azure App Configuration** integration for centralized management
- **User targeting** filters for specific user segments
- **Browser/device** filters for responsive features
- **Regional** filters for geo-specific features
- **Telemetry** integration to track feature usage
- **Feature flag lifecycle** management and cleanup
