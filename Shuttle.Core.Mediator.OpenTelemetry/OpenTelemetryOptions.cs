namespace Shuttle.Core.Mediator.OpenTelemetry
{
    public class MediatorOpenTelemetryOptions
    {
        public const string SectionName = "Shuttle:Instrumentation:Mediator";

        public bool Enabled { get; set; } = true;
        public bool IncludeSerializedMessage { get; set; } = true;
    }
}