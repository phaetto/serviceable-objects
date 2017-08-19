
# Serviceable Objects

It's a library about object servicing and connectivity from different domains.
The main focus is on testability for the producing code as well as composable services from the developed subcomponents.

The base library (Serviceable.Objects) is made for .NET standard 1.2+.
Remote library (Serviceable.Objects.Remote) is made for .NET standard 1.3+.
If you would like to resolve types in assemblies without assembly qualified naming, then you have to use at least .net standard 1.6+ or .NET 4.5.1+.

## Concept
I am considering "a serviceable object" as a service, tight to the lifetime of an object.
When the object that describes the service is disposed, then the service itself should be considered shut down.

### Context
The smallest building block of this library is named Context : this can be anything, even a servicable object; it's the context that we want to apply an action.
Context concept allows us to build around it mechanics that could be applied to any object that acts as an API.

On a context we can apply commands; this is your trivial everyday command pattern.
Those commands should include all the necessary information to run the command at some later point.

On every command execution there is the possibility to publish events, driven by each command, that make sense to the system implemented.

So the basic block of the system looks like this:
![alt text](https://raw.githubusercontent.com/phaetto/serviceable-objects/master/images/theory-commands.png "How the interaction looks")

By following the good practices, all parts of this construct should also be unit testable.

### Graph
The ultimate problem on that micro-API architecture is that when we combine services,
it is very hard to find a pattern that all apis could use to tackle composability by configuration
(this is what this library aims to solve)

So between the contexts an event translation mechanism is introduced and this glues together a graph of micro-API contexts running together.

And so on top of those we are composing services with different context parts that have more specific functionality:
servers, queues, configurations, clients, etc. We can now create a graph of contexts that communicate with the events that they generate:
![alt text](https://raw.githubusercontent.com/phaetto/serviceable-objects/master/images/theory-composable.png "Composable context - the context graph")

#### Example

Theory: Commands -> Context + Events
- Examples

Mechanics
- Chainable
- Async/Sync
- Type proxies
- Events
- Reproducibility/Remotability
- Extras

Composition
- Why composition?
- Graph mechanics
- Example