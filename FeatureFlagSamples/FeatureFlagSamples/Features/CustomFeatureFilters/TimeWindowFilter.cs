using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration;

namespace FeatureFlagSamples.Features.CustomFeatureFilters;

/// <summary>
/// Custom feature filter that enables a feature only within a specified time window.
/// Demonstrates how to create custom feature evaluation logic.
/// </summary>
[FilterAlias("CustomTimeWindow")]
public class TimeWindowFilter : IFeatureFilter
{
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        var startValue = context.Parameters.GetValue<string>("Start");
        var endValue = context.Parameters.GetValue<string>("End");

        if (string.IsNullOrEmpty(startValue) || string.IsNullOrEmpty(endValue))
        {
            return Task.FromResult(false);
        }

        if (!DateTimeOffset.TryParse(startValue, out var start) || 
            !DateTimeOffset.TryParse(endValue, out var end))
        {
            return Task.FromResult(false);
        }

        var now = DateTimeOffset.UtcNow;
        var isEnabled = now >= start && now <= end;

        return Task.FromResult(isEnabled);
    }
}
