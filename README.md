# Shuttle.Core.Mediator.OpenTelemetry

```
PM> Install-Package Shuttle.Core.Mediator
```

OpenTelemetry instrumentation for Shuttle.Core.Mediator implementations.

## Configuration

```c#
services.AddServiceBusInstrumentation(builder =>
{
	// default values
    builder.Options.Enabled = true;
    builder.Options.IncludeSerializedMessage = true;

	// or bind from configuration
	configuration
		.GetSection(OpenTelemetryOptions.SectionName)
		.Bind(builder.Options);
});


## Options

| Option | Default	| Description |
| --- | --- | --- | 
| `Enabled` | `true` | Indicates whether to perform instrumentation. |
| `IncludeSerializedMessage` | `true` | If 'true', includes the serialized message as attribute `SerializedMessage` in the trace. |