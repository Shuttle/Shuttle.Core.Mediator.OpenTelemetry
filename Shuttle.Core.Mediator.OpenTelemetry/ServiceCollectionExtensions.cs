using System;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Core.Mediator.OpenTelemetry
{
    public static class ServiceCollectionExtensions
    {
        public static TracerProviderBuilder AddMediatorInstrumentation(this TracerProviderBuilder tracerProviderBuilder, Action<OpenTelemetryBuilder> builder = null)
        {
            Guard.AgainstNull(tracerProviderBuilder, nameof(tracerProviderBuilder));

            tracerProviderBuilder.AddSource("Shuttle.Core.Mediator");

            tracerProviderBuilder.ConfigureServices(services =>
            {
                var openTelemetryBuilder = new OpenTelemetryBuilder(services);

                builder?.Invoke(openTelemetryBuilder);

                services.AddOptions<MediatorOpenTelemetryOptions>().Configure(options =>
                {
                    options.Enabled = openTelemetryBuilder.Options.Enabled;
                });

                services.AddPipelineModule<OpenTelemetryModule>();
            });

            return tracerProviderBuilder;
        }
    }
}