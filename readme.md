# Serviceable Objects

A library that attempts to lower the total complexity of a system by introducing a layer of objects-like-services.

Instead of the system objects to call each stage by wrapping them in functions, this library provides a layer of a bus-like system that provides the communication with each object (that acts as a kind of internal long running service). The responsibility of each of those objects is to use commands and events as a service, to provide the signals and the means programmatically and dynamically to the whole system of objects.

Concepts
- *Contexts:*

Contexts are the heart of the system. They provide the necessary abstraction to allow commands to apply on them, either synchronous or asynchronous, and they provide a chainable developer experience.

- *Commands:*

Commands are the immutable objects that direct a change or some kind of business logic on a specific context. Commands can be modelled decoupled from their data. This allows commands to be transferred across domains when the respective bits are included in both sides.

- ‎*Events:*

Events are a byproduct of a command being applied to a context. It's the resulting signals that something happens and can be followed up from other listeners.

- *Graphs:*

Graphs provide the composition of a system that consists from contexts. When commands applied to the graph, events flow in a consistent and predictive manner. Contexts that are connected together can react to the events and provide specific business logic depending the scenario.

- *Services:*

Services are an encapsulation of Graphs that can be described uniquely across a large microservices system.

- ‎*Service containers*:

The container of many services. Shares the process space.

- ‎*Services orchestrator*:

The responsible part that orchestrates the start, stop and containment of a service.

- *Application host*:

The runtime connection to a process. A developer can customise it in a custom process.

## Versions

Project | Description | Supporting Platform
--- | --- | ---
Serviceable.Objects | The main Context/Command/Event/Graph library | .NET Standard 1.3
Serviceable.Objects.Remote | Everything that has to do with remotables, JSON and dynamicity | .NET Standard 1.3*
Serviceable.Objects.IO | Streaming and remoting support | .NET Standard 1.3
Serviceable.Objects.Instrumentation | Powershell integration and instrumentation | .NET Standard 1.6

*Note: if assembly searching is required, it needs to support at least .NET Standard 1.6 or .NET Framework 4.6

## If you have a comment

Feel free to contact me at:
Alexander Mantzoukas - alexander.mantzoukas@gmail.com

## License

Licensed under MIT.