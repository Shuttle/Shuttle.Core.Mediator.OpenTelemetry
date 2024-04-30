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

	// or bind from configuration
	configuration
		.GetSection(MediatorOpenTelemetryOptions.SectionName)
		.Bind(builder.Options);
});


## Options

| Option | Default	| Description |
| --- | --- | --- | 
| `Enabled` | `true` | Indicates whether to perform instrumentation. |