using Observability.Constants;
using OpenTelemetry.Trace;

namespace Observability.Extensions;

internal static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddCustomEvent(this TracerProviderBuilder builder)
    {
        return builder.AddSource(ActivitySourceConstants.CustomEventName);
    }
}
