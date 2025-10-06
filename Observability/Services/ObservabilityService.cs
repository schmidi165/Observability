using Observability.Constants;
using System.Diagnostics;

namespace Observability.Services;

public sealed class ObservabilityService
{
    private static readonly ActivitySource activitySource = new(ActivitySourceConstants.CustomEventName);

    public async Task TestLongRunningOperation(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

        using (var activity = activitySource.StartActivity("TestLongRunningOperation"))
        {
            activity?.SetTag("customTag", "customValue");

            activity?.AddEvent(new("Starting long-running operation"));

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            activity?.AddEvent(new("Halfway through long-running operation"));

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            activity?.AddEvent(new("Almost done with long-running operation"));
        }

        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
    }
}
