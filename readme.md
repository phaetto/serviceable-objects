
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
servers, queues, configurations, clients, etc. We can now create a graph of contexts that communicate with the events that they generate.

The graph looks like this:
![alt text](https://raw.githubusercontent.com/phaetto/serviceable-objects/master/images/theory-composable.png "Composable context - the context graph")

#### Example
Let's take as an example a queue service. We would like to create an http-based queue service that can enqueue/dequeue text messages.
We will try to make that composable and see what benefits that might have.

We define our base context (this is our serviceable object for this example - the rest will be supportive functionality):
```csharp
namespace TestHttpCompositionConsoleApp.Contexts.Queues
{
    using System.Collections.Generic;
    using Serviceable.Objects;
    using TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Data;

    public sealed class QueueContext : Context<QueueContext>
    {
        public Queue<QueueItem> QueueStorage = new Queue<QueueItem>();
    }
}
```

A very simple queue context that just wraps a Queue.
Next we need the two commands - let's try to implement the enqueue first.

Now, we are building an _http_ service, so we will need to consider the transport of the commands.
In this case we are going to use the Reproducible line of classes, and more specifically the [ReproducibleCommandWithData](https://github.com/phaetto/serviceable-objects/blob/master/Serviceable.Objects.Remote/RemotableCommandWithData.cs).

```csharp
namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands
{
    using Serviceable.Objects.Remote;

    public sealed class Enqueue : ReproducibleCommandWithData<QueueContext, QueueContext, ???>
    {
    }
}
```

As we can see from above this means that we need to explicitly need to provide a POCO object that will act as our data:

```csharp
namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Data
{
    using Serviceable.Objects.Remote.Serialization;

    public sealed class QueueItem : SerializableSpecification
    {
        public override int DataStructureVersionNumber => 1;

        public string Data { get; set; }
    }
}
```

We will not care about the data-version management right now, so we will leave it at 1.

The line ```string Data``` is our data that will be added to the queue. Sweet. Let's go back on our enqueue class:

```csharp
namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands
{
    using Serviceable.Objects.Remote;
    using TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Data;

    public sealed class Enqueue : ReproducibleCommandWithData<QueueContext, QueueContext, QueueItem>
    {
        public Enqueue(QueueItem data) : base(data)
        {
        }

        public override QueueContext Execute(QueueContext context)
        {
            context.QueueStorage.Enqueue(Data);
            return context;
        }
    }
}
```

The data will need to passed on the constructor and the Execute function will need to be implemented.
We do not want to return anything specific here, so we just return the same context.

The enqueue behavior can now be used in a unit-test:
```
var queue = new QueueContext();
queue.Execute(new Enqueue(new QueueItem { Data = "data-1" }))
    .Execute(new Enqueue(new QueueItem { Data = "data-2" }))
    .Execute(new Enqueue(new QueueItem { Data = "data-3" }));
Assert.Equal(3, queue.QueueStorage.Count);
```

When we make sure that this command works as expected, we move forward.
Let's go to our next command, Dequeue. This command has the need to _return_ value to the caller.
And so we will derive our command from the [RemotableCommandWithData](https://github.com/phaetto/serviceable-objects/blob/master/Serviceable.Objects.Remote/RemotableCommandWithData.cs):

```csharp
namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands
{
    using Serviceable.Objects.Remote;
    using TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Data;

    public sealed class Dequeue : RemotableCommand<QueueItem, QueueContext>
    {
        public override QueueItem Execute(QueueContext context)
        {
            if (context.QueueStorage.Count == 0)
            {
                return null;
            }

            return context.QueueStorage.Dequeue();
        }
    }
}
```
So, now when we dequeue we should get a QueueItem. We can test that as well:
```
var queue = new QueueContext();
var queueItem = queue.Execute(new Enqueue(new QueueItem { Data = "data" }))
    .Execute(new Dequeue());
Assert.Equal("data", queueItem.Data);
```

Great! We have now our Queue service running and transportable commands.

Now we need the http server. We will use owin for that and create another simple context:
```csharp
namespace TestHttpCompositionConsoleApp.Contexts.Http
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Serviceable.Objects;
    using Serviceable.Objects.Remote.Serialization;

    public sealed class OwinHttpContext : Context<OwinHttpContext>
    {
        public readonly IWebHost Host;

        public OwinHttpContext()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables().Build();

            Host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(l => l.AddConsole())
                .ConfigureServices(s => s.AddRouting())
                .Configure(app =>
                {
                    app.UseRouter(SetupRouter);
                })
                .Build();
        }

        private void SetupRouter(IRouteBuilder routerBuilder)
        {
            routerBuilder.MapPost("test", TestRequestHandler);
        }

        private async Task TestRequestHandler(HttpContext context)
        {
            string data;
            using (var streamReader = new StreamReader(context.Request.Body))
            {
                data = streamReader.ReadToEnd();
            }

            var spec = DeserializableSpecification<ExecutableCommandSpecification>.DeserializeFromJson(data);
            var command = spec.CreateFromSpec();

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject("json text"));
        }
    }
}

```

I am not going to go through all details, this is a server that works on localhost:5000/test endpoint.
The server gets the data through ```TaskRequestHandler``` and transforms it to an object with the help of ```DeserializableSpecification``` class.

Now we have to look again on the mechanics of the server.
On the previous graph figure we actually assumed that the graph starts the execution from a command.
This is not true however for many cases. There is the need, like in our server, to pass the control to the context running,
and this in turn should signal the graph node to continue the propagation of the execution.

It looks like this in out case:
![alt text](https://raw.githubusercontent.com/phaetto/serviceable-objects/master/images/theory-composable-http.png "Composable context - server controls the flow internally")

This is why we have some custom event implementations like IGraphFlowEventPushControl.
This interface creates a factory on the custom event that we can publish and transforms the event to execution of a command internally in the graph.

We have already an event that does that [GraphFlowEventPushControlApplyCommandInsteadOfEvent](https://github.com/phaetto/serviceable-objects/blob/master/Serviceable.Objects/Composition/Events/GraphFlowControlApplyCommand.cs), so let's use it:
```csharp
...
private async Task TestRequestHandler(HttpContext context)
{
    string data;
    using (var streamReader = new StreamReader(context.Request.Body))
    {
        data = streamReader.ReadToEnd();
    }

    var spec = DeserializableSpecification<ExecutableCommandSpecification>.DeserializeFromJson(data);
    var command = spec.CreateFromSpec();
            
    var eventResults =
        OnCommandEventWithResultPublished(new GraphFlowEventPushControlApplyCommandInsteadOfEvent(command))
        .Where(x => x.ResultObject != null).ToList();

    if (eventResults.Count > 0)
    {
        var results = eventResults.Select(x => x.ResultObject);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(results));
    }
    else
    {
        context.Response.StatusCode = 204;
    }
}
...
```

So we signal the application that we need it to run a command and the next services in the graph will try to apply this command.

The we get the results, and we serialize them as our http output.

Let's now make out graph in a console app:
```csharp
namespace TestHttpCompositionConsoleApp
{
    using Serviceable.Objects.Composition;
    using Serviceable.Objects.Remote.Composition;
    using TestHttpCompositionConsoleApp.Contexts.ConsoleLog;
    using TestHttpCompositionConsoleApp.Contexts.Http;
    using TestHttpCompositionConsoleApp.Contexts.Http.Commands;
    using TestHttpCompositionConsoleApp.Contexts.Queues;

    class Program
    {
        static void Main(string[] args)
        {
            var configuration = @"
{
    GraphVertices: [
        { TypeFullName:'" + typeof(OwinHttpContext).FullName + @"', Id:'server-context' },
        { TypeFullName:'" + typeof(QueueContext).FullName + @"', Id:'queue-context', ParentId:'server-context' },
    ]
}
";
            
            var contextGraph = new ContextGraph();
            contextGraph.FromJson(configuration);
            contextGraph.Execute(new Run());
        }
    }
}
```

And that's it! Now when we start the application we should eb able to post at localhost:5000/test something like:

```json
{
	Type: "TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Enqueue",
	DataType: "TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Data.QueueItem",
	Data: {
		Data: "some data"
	}
}
```

or

```json
{
	Type: "TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Dequeue",
}
```

And get add or remove elements from the queue.

#### Composability



## Thanks!

If you have any suggestion or comment:
Alexander Mantzoukas - alexander.mantzoukas@gmail.com