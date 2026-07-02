# Feature Flag Implementation Guide

## 📋 Solution Overview

This solution provides a **production-ready demonstration** of feature flag patterns in .NET 10 using Microsoft.FeatureManagement. It showcases best practices for implementing, organizing, and managing feature flags in C# applications.

## 🏗️ Architecture

### Project Structure

```
FeatureFlagSamples/
├── 📁 Features/
│   ├── FeatureFlags.cs                      # Centralized feature flag constants
│   └── 📁 CustomFeatureFilters/
│       └── TimeWindowFilter.cs              # Custom time-based feature filter
│
├── 📁 Services/
│   ├── INotificationService.cs              # Service abstraction
│   ├── BasicNotificationService.cs          # Standard implementation
│   ├── AdvancedNotificationService.cs       # Enhanced implementation
│   └── FeatureAwareNotificationService.cs   # Runtime feature evaluation
│
├── 📁 Scenarios/
│   ├── BasicFeatureFlagScenario.cs          # Simple boolean toggles
│   ├── ConditionalDependencyScenario.cs     # DI-based switching
│   ├── PercentageRolloutScenario.cs         # Gradual rollouts
│   ├── TimeWindowScenario.cs                # Time-based activation
│   └── FeatureVariantScenario.cs            # A/B/C testing
│
├── 📁 Extensions/
│   └── ServiceCollectionExtensions.cs       # DI configuration
│
├── ⚙️ appsettings.json                      # Production configuration
├── ⚙️ appsettings.Development.json          # Development overrides
├── 📄 Program.cs                            # Application entry point
└── 📖 README.md                             # Documentation
```

## 🎯 Key Patterns Demonstrated

### 1️⃣ Centralized Feature Flag Names

**Problem:** String literals scattered throughout code lead to typos and make refactoring difficult.

**Solution:** Use constants in a central class.

```csharp
public static class FeatureFlags
{
	public const string NewUI = "NewUI";
	public const string BetaFeatures = "BetaFeatures";
	public const string AdvancedLogging = "AdvancedLogging";
}
```

**Benefits:**
- ✅ Compile-time checking
- ✅ IntelliSense support
- ✅ Safe refactoring
- ✅ Easy discovery of all flags

### 2️⃣ Configuration-Based Management

**appsettings.json:**
```json
{
  "FeatureManagement": {
	"NewUI": true,
	"BetaFeatures": false,
	"AdvancedLogging": true
  }
}
```

**Benefits:**
- ✅ No code changes needed
- ✅ Environment-specific overrides
- ✅ Runtime reloading (when configured)
- ✅ Easy deployment pipeline integration

### 3️⃣ Runtime Feature Evaluation

**Pattern:** Check features at runtime for dynamic behavior

```csharp
if (await _featureManager.IsEnabledAsync(FeatureFlags.NewUI))
{
	// Show new UI
}
else
{
	// Show classic UI
}
```

**Use Cases:**
- Conditional UI rendering
- Algorithm selection
- Feature access control
- Logging verbosity

### 4️⃣ Dependency Injection Based on Flags

**Pattern:** Register different service implementations based on feature state

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

**Benefits:**
- ✅ Clean separation of implementations
- ✅ No runtime conditionals in business logic
- ✅ Easy A/B testing
- ✅ Simplified rollback

**Limitation:** Evaluated at startup only. For runtime switching, inject `IFeatureManager` into the service.

### 5️⃣ Percentage-Based Gradual Rollout

**Configuration:**
```json
{
  "DatabaseMigration": {
	"EnabledFor": [
	  {
		"Name": "Percentage",
		"Parameters": { "Value": 50 }
	  }
	]
  }
}
```

**Use Cases:**
- Canary deployments
- Gradual feature rollout
- Load testing new features
- Risk mitigation

**How it works:** Uses consistent hashing to ensure the same user/context always gets the same result.

### 6️⃣ Custom Feature Filters

**Pattern:** Create custom evaluation logic for complex scenarios

```csharp
[FilterAlias("CustomTimeWindow")]
public class TimeWindowFilter : IFeatureFilter
{
	public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
	{
		var start = context.Parameters.GetValue<string>("Start");
		var end = context.Parameters.GetValue<string>("End");

		// Custom time-based logic
		var now = DateTimeOffset.UtcNow;
		return Task.FromResult(now >= start && now <= end);
	}
}
```

**Configuration:**
```json
{
  "PremiumFeatures": {
	"EnabledFor": [
	  {
		"Name": "CustomTimeWindow",
		"Parameters": {
		  "Start": "2024-01-01T00:00:00Z",
		  "End": "2025-12-31T23:59:59Z"
		}
	  }
	]
  }
}
```

**Use Cases:**
- Scheduled feature releases
- Promotional periods
- Maintenance windows
- Business hours restrictions
- Geographic targeting (custom filter needed)
- User segment targeting (custom filter needed)

## 🔧 Built-in Feature Filters

Microsoft.FeatureManagement provides several built-in filters:

1. **PercentageFilter** - Gradual rollout by percentage
2. **TimeWindowFilter** - Enable during specific time periods
3. **TargetingFilter** - User/group targeting (requires additional package)
4. **ContextualTargetingFilter** - Advanced user targeting

## 🚀 Running the Application

### Build and Run

```bash
# Build the solution
dotnet build

# Run with production settings
dotnet run --project FeatureFlagSamples\FeatureFlagSamples.csproj

# Run with development settings
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project FeatureFlagSamples\FeatureFlagSamples.csproj
```

### Expected Output

The application demonstrates 5 scenarios:
1. ✅ **Basic Feature Flags** - Simple on/off toggles
2. ✅ **Conditional Dependency** - Service implementation switching
3. ✅ **Percentage Rollout** - Gradual feature deployment
4. ✅ **Time Window** - Scheduled feature availability
5. ✅ **Feature Variants** - A/B/C testing patterns

## 📝 Best Practices

### ✅ DO

1. **Use centralized constants** for feature flag names
   ```csharp
   // Good
   await _featureManager.IsEnabledAsync(FeatureFlags.NewUI);

   // Bad
   await _featureManager.IsEnabledAsync("NewUI");
   ```

2. **Document what each flag controls**
   ```csharp
   /// <summary>
   /// Enables the new React-based user interface.
   /// When disabled, uses the legacy ASP.NET MVC UI.
   /// </summary>
   public const string NewUI = "NewUI";
   ```

3. **Remove flags after full rollout**
   - Once a feature is 100% enabled in production, remove the flag
   - Delete the old code path
   - Clean up configuration

4. **Use environment-specific configurations**
   - `appsettings.Development.json` for dev
   - `appsettings.Production.json` for prod
   - Environment variables for runtime overrides

5. **Log feature flag evaluations** for debugging
   ```csharp
   var isEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.NewUI);
   _logger.LogDebug("Feature {FeatureName} is {Status}", 
	   FeatureFlags.NewUI, isEnabled ? "enabled" : "disabled");
   ```

6. **Test both enabled and disabled states**
   - Unit tests for both code paths
   - Integration tests with different configurations

7. **Use feature filters for complex scenarios**
   - Percentage rollouts
   - Time-based activation
   - User/group targeting

### ❌ DON'T

1. **Don't use magic strings**
   ```csharp
   // Bad - prone to typos
   if (await _featureManager.IsEnabledAsync("newUI"))
   ```

2. **Don't leave flags indefinitely**
   - Feature flags are technical debt
   - Set a removal date when creating a flag
   - Review and clean up regularly

3. **Don't nest flags too deeply**
   ```csharp
   // Bad - complex and hard to understand
   if (await _featureManager.IsEnabledAsync(FeatureFlags.NewUI))
   {
	   if (await _featureManager.IsEnabledAsync(FeatureFlags.BetaFeatures))
	   {
		   if (await _featureManager.IsEnabledAsync(FeatureFlags.ExperimentalMode))
		   {
			   // Too complex!
		   }
	   }
   }
   ```

4. **Don't use flags for configuration values**
   ```csharp
   // Bad - use Configuration instead
   "ConnectionString": { "EnabledFor": [...] }

   // Good - feature flags should be boolean decisions
   "UseLegacyDatabase": true
   ```

5. **Don't forget to handle both states**
   ```csharp
   // Bad - what if the flag is disabled?
   if (await _featureManager.IsEnabledAsync(FeatureFlags.NewUI))
   {
	   ShowNewUI();
   }
   // No else branch!

   // Good
   if (await _featureManager.IsEnabledAsync(FeatureFlags.NewUI))
   {
	   ShowNewUI();
   }
   else
   {
	   ShowClassicUI();
   }
   ```

## 🎓 Advanced Scenarios

### Feature Flag Lifecycle

1. **Development** → Flag created, defaults to `false`
2. **Testing** → Enabled in dev/test environments
3. **Canary** → Enabled for 5-10% of production users
4. **Ramp-up** → Gradually increase to 25%, 50%, 75%
5. **Full Rollout** → Enabled for 100% of users
6. **Cleanup** → Remove flag and old code path

### Integration with Azure App Configuration

For enterprise scenarios, integrate with Azure App Configuration:

```csharp
services.AddAzureAppConfiguration();
services.AddFeatureManagement()
		.WithTargeting<TargetingContext>();
```

Benefits:
- Centralized management across environments
- Real-time updates without redeployment
- Advanced targeting rules
- Feature flag analytics
- Audit logging

### Monitoring and Telemetry

Track feature flag usage:

```csharp
var isEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.NewUI);
_telemetry.TrackEvent("FeatureFlagEvaluated", new Dictionary<string, string>
{
	{ "FeatureName", FeatureFlags.NewUI },
	{ "IsEnabled", isEnabled.ToString() },
	{ "UserId", context.UserId }
});
```

## 📚 Additional Resources

- [Microsoft.FeatureManagement GitHub](https://github.com/microsoft/FeatureManagement-Dotnet)
- [Azure App Configuration Feature Flags](https://learn.microsoft.com/azure/azure-app-configuration/concept-feature-management)
- [Feature Toggle Patterns - Martin Fowler](https://martinfowler.com/articles/feature-toggles.html)
- [Feature Flags Best Practices](https://learn.microsoft.com/azure/azure-app-configuration/howto-best-practices#feature-flag-management)

## 🔮 Future Enhancements

This sample can be extended with:

- [ ] Azure App Configuration integration
- [ ] User targeting filters with audience segmentation
- [ ] Browser/device detection filters
- [ ] Geographic/regional filters
- [ ] Application Insights telemetry integration
- [ ] Admin UI for managing feature flags
- [ ] Feature flag lifecycle tracking
- [ ] Automated cleanup of expired flags
- [ ] Performance benchmarking scenarios
- [ ] ASP.NET Core middleware integration
- [ ] Blazor component examples

## 🤝 Contributing

When adding new feature flag patterns:

1. Create a new scenario in the `Scenarios/` folder
2. Document the pattern in code comments
3. Add configuration examples to `appsettings.json`
4. Update this guide with the new pattern
5. Ensure both enabled/disabled states work correctly
6. Add appropriate logging

---

**Remember:** Feature flags are a powerful tool, but they add complexity. Use them judiciously and clean them up promptly!
