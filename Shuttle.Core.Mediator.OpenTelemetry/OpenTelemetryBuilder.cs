using System;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Mediator.OpenTelemetry
{
    public class OpenTelemetryBuilder
    {
        private MediatorOpenTelemetryOptions _openTelemetryOptions = new MediatorOpenTelemetryOptions();

        public OpenTelemetryBuilder(IServiceCollection services)
        {
            Services = Guard.AgainstNull(services, nameof(services));
        }

        public MediatorOpenTelemetryOptions Options
        {
            get => _openTelemetryOptions;
            set => _openTelemetryOptions = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IServiceCollection Services { get; }
    }
}