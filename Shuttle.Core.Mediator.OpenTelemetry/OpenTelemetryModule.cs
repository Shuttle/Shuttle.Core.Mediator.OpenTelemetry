using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly MediatorOpenTelemetryOptions _openTelemetryOptions;
        private readonly Tracer _tracer;
        private readonly Dictionary<Guid, TelemetrySpan> _telemetrySpans = new Dictionary<Guid, TelemetrySpan>();

        public OpenTelemetryModule(IOptions<MediatorOpenTelemetryOptions> openTelemetryOptions, TracerProvider tracerProvider, IMediator mediator, ISerializer serializer = null)
        {
            Guard.AgainstNull(openTelemetryOptions, nameof(openTelemetryOptions));
            Guard.AgainstNull(openTelemetryOptions.Value, nameof(openTelemetryOptions.Value));
            Guard.AgainstNull(tracerProvider, nameof(tracerProvider));
            Guard.AgainstNull(mediator, nameof(mediator));

            _openTelemetryOptions = openTelemetryOptions.Value;

            if (!_openTelemetryOptions.Enabled)
            {
                return;
            }

            _tracer = tracerProvider.GetTracer("Shuttle.Core.Mediator");
            _mediator = mediator;
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

            try
            {
                if (_openTelemetryOptions.IncludeSerializedMessage && _serializer != null)
                {
                    using (var stream = _serializer.Serialize(e.Message))
                    using (var reader = new StreamReader(stream))
                    {
                        telemetrySpan?.SetAttribute("SerializedMessage", reader.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
                telemetrySpan?.SetAttribute("SerializedMessage", ex.Message);
            }

            telemetrySpan?.SetAttribute("MessageType", e.Message.GetType().FullName);

            _telemetrySpans.Add(e.Id, telemetrySpan);
        }
    }
}