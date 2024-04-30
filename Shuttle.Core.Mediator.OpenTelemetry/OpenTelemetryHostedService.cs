using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using Shuttle.Core.Contract;
using Shuttle.Core.Serialization;

namespace Shuttle.Core.Mediator.OpenTelemetry
{
    public class OpenTelemetryHostedService : IHostedService
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<Guid, TelemetrySpan> _telemetrySpans = new Dictionary<Guid, TelemetrySpan>();
        private readonly Tracer _tracer;
        private readonly MediatorOpenTelemetryOptions _openTelemetryOptions;

        public OpenTelemetryHostedService(IOptions<MediatorOpenTelemetryOptions> openTelemetryOptions, TracerProvider tracerProvider, IMediator mediator, ISerializer serializer = null)
        {
            Guard.AgainstNull(openTelemetryOptions, nameof(openTelemetryOptions));
            
            _openTelemetryOptions = Guard.AgainstNull(openTelemetryOptions.Value, nameof(openTelemetryOptions.Value));

            if (!_openTelemetryOptions.Enabled)
            {
                return;
            }

            _tracer = Guard.AgainstNull(tracerProvider, nameof(tracerProvider)).GetTracer("Shuttle.Core.Mediator");
            _mediator = Guard.AgainstNull(mediator, nameof(mediator));

            _mediator.Sending += Sending;
            _mediator.Sent += Sent;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_openTelemetryOptions.Enabled)
            {
                _mediator.Sending -= Sending;
                _mediator.Sent -= Sent;
            }

            return Task.CompletedTask;
        }

        private void Sending(object sender, SendEventArgs e)
        {
            Guard.AgainstNull(sender, nameof(sender));
            Guard.AgainstNull(e, nameof(e));

            var telemetrySpan = _tracer.StartActiveSpan("Send");

            telemetrySpan.SetAttribute("MessageType", e.Message.GetType().FullName);

            _telemetrySpans.Add(e.Id, telemetrySpan);
        }

        private void Sent(object sender, SendEventArgs e)
        {
            Guard.AgainstNull(sender, nameof(sender));
            Guard.AgainstNull(e, nameof(e));

            if (!_telemetrySpans.TryGetValue(e.Id, out var telemetrySpan))
            {
                return;
            }

            _telemetrySpans.Remove(e.Id);

            telemetrySpan?.End();
            telemetrySpan?.Dispose();
        }
    }
}