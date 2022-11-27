using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using Shuttle.Core.Contract;
using Shuttle.Core.Serialization;

namespace Shuttle.Core.Mediator.OpenTelemetry
{
    public class OpenTelemetryModule : IDisposable
    {
        private readonly IMediator _mediator;
        private readonly ISerializer _serializer;

        private readonly Tracer _tracer;
        private readonly Dictionary<Guid, TelemetrySpan> _telemetrySpans = new Dictionary<Guid, TelemetrySpan>();

        public OpenTelemetryModule(IOptions<MediatorOpenTelemetryOptions> openTelemetryOptions, TracerProvider tracerProvider, IMediator mediator, ISerializer serializer = null)
        {
            Guard.AgainstNull(openTelemetryOptions, nameof(openTelemetryOptions));
            Guard.AgainstNull(openTelemetryOptions.Value, nameof(openTelemetryOptions.Value));
            
            if (!openTelemetryOptions.Value.Enabled)
            {
                return;
            }

            _tracer = Guard.AgainstNull(tracerProvider, nameof(tracerProvider)).GetTracer("Shuttle.Core.Mediator");
            _mediator = Guard.AgainstNull(mediator, nameof(mediator));
            _serializer = serializer;

            _mediator.Sending += Sending;
            _mediator.Sent += Sent;
        }

        public void Dispose()
        {
            _mediator.Sending -= Sending;
            _mediator.Sent-= Sent;
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

        private void Sending(object sender, SendEventArgs e)
        {
            Guard.AgainstNull(sender, nameof(sender));
            Guard.AgainstNull(e, nameof(e));

            var telemetrySpan = _tracer.StartActiveSpan("Send");

            telemetrySpan?.SetAttribute("MessageType", e.Message.GetType().FullName);

            _telemetrySpans.Add(e.Id, telemetrySpan);
        }
    }
}