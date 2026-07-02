# Quick Start Guide

## 🚀 Get Started in 2 Minutes

### 1. Run the Demo

```bash
dotnet run --project FeatureFlagSamples\FeatureFlagSamples.csproj
```

You'll see 5 demonstration scenarios showing different feature flag patterns.

### 2. Try Changing a Flag

Edit `appsettings.json`:

```json
{
  "FeatureManagement": {
	"BetaFeatures": true   // Change this to true
  }
}
```

Run again and notice how the notification service changes from Basic to Advanced!

### 3. Test Development Mode

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project FeatureFlagSamples\FeatureFlagSamples.csproj
```

This loads `appsettings.Development.json` where more features are enabled.

## 📖 What to Explore

### Example 1: Simple Boolean Toggle

**File:** `Scenarios/BasicFeatureFlagScenario.cs`

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

### Example 2: Service Dependency Injection

**File:** `Extensions/ServiceCollectionExtensions.cs`

Shows how to register different service implementations based on feature flags.

### Example 3: Gradual Rollout

**File:** `Scenarios/PercentageRolloutScenario.cs`

Demonstrates 50% rollout using the built-in Percentage filter.

### Example 4: Custom Filter

**File:** `Features/CustomFeatureFilters/TimeWindowFilter.cs`

Shows how to create custom feature evaluation logic.

## 🎯 Key Files to Review

| File | Purpose |
|------|---------|
| `Features/FeatureFlags.cs` | Central feature flag constants |
| `appsettings.json` | Feature flag configuration |
| `Program.cs` | Application setup and all scenarios |
| `Services/*.cs` | Different service implementations |
| `Scenarios/*.cs` | 5 demonstration patterns |

## 💡 Common Patterns

### Check a Feature Flag

```csharp
var isEnabled = await _featureManager.IsEnabledAsync(FeatureFlags.NewUI);
```

### Add a New Feature Flag

1. Add constant to `Features/FeatureFlags.cs`:
   ```csharp
   public const string MyFeature = "MyFeature";
   ```

2. Add to `appsettings.json`:
   ```json
   {
	 "FeatureManagement": {
	   "MyFeature": true
	 }
   }
   ```

3. Use in code:
   ```csharp
   if (await _featureManager.IsEnabledAsync(FeatureFlags.MyFeature))
   {
	   // Feature code
   }
   ```

### Percentage Rollout

```json
{
  "FeatureManagement": {
	"MyFeature": {
	  "EnabledFor": [
		{
		  "Name": "Percentage",
		  "Parameters": { "Value": 25 }
		}
	  ]
	}
  }
}
```

## 📚 Next Steps

1. ✅ Run the demo
2. ✅ Modify `appsettings.json` and see changes
3. ✅ Review the code in `Scenarios/`
4. ✅ Read `IMPLEMENTATION_GUIDE.md` for detailed patterns
5. ✅ Read `README.md` for architecture overview

## 🎓 Learn More

- **Best Practices:** See `IMPLEMENTATION_GUIDE.md`
- **Architecture:** See `README.md`
- **Official Docs:** [Microsoft.FeatureManagement](https://github.com/microsoft/FeatureManagement-Dotnet)

---

**Happy feature flagging! 🚩**
