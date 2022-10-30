using System;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Core.Mediator.OpenTelemetry
{
    public static class ServiceCollectionExtensions
    {
        public static TracerProviderBuilder AddMediatorSource(this TracerProviderBuilder builder)
        {
            Guard.AgainstNull(builder, nameof(builder));

            builder.AddSource("Shuttle.Core.Mediator");

            return builder;
        }

        public static IServiceCollection AddMediatorInstrumentation(this IServiceCollection services,
            Action<OpenTelemetryBuilder> builder = null)
        {
            Guard.AgainstNull(services, nameof(services));

            var openTelemetryBuilder = new OpenTelemetryBuilder(services);

            builder?.Invoke(openTelemetryBuilder);

            services.AddOptions<MediatorOpenTelemetryOptions>().Configure(options =>
            {
                options.Enabled = openTelemetryBuilder.Options.Enabled;
                options.IncludeSerializedMessage = openTelemetryBuilder.Options.IncludeSerializedMessage;
            });

            services.AddPipelineModule<OpenTelemetryModule>();

            return services;
        }
    }
}