# Disclaimer

This repo is a copy of a project I did some years ago on
[CodePlex](https://archive.codeplex.com/?p=asyncexeclib). I'm no longer
maintaining the project, as I've switched to Java development in my professional
career years ago and my C# skills became rusty as a result. Many of my learnings
in this project have moved into the [saga-lib](https://github.com/Domo42/saga-lib)
library.

While no longer actively maintained, having the code on GitHub might still be
useful to some.

# Project Description

This library simplifies the process of executing code on a different thread
and separating the caller from the actual command logic. To do this messages
are put into an execution module and the library automatically calls the target
message handlers.

The goal of the library is to have a working module with little to no
configuration. In this way it is is greatly inspired by NServiceBus. This
library has a similar feel but works purely in-process. The engine automatically
calls message handlers which handle either a specific message, a base class or
interface. Multiple handlers may exist for the same message. For simple tasks
there is also an overload to execute an Action delegate. Message handlers are
either executed on a single worker thread or on a custom number of workers.

The library is using dependency injection. This allows for an easy way to add
external dependencies into your message handlers to perform their task.
Additionally, it makes it easy to tweak and extend the library without having
to edit the source itself.

## Basic Usage:
Using the library is simple. First define a message to be used. This can be any
.NET class. (In previous versions messages had to inherit from IMessage).

```csharp
public class CreateReportMessage
{
  public string Title { get; set; }
}
```

Next create a message handler. This is achieved by inheriting a class from
```IMessageHandler<T>```. This handler will be created and called every time a
message is received matching the handler definition.

```csharp
public class CreateReportHandler : IMessageHandler<CreateReportMessage>
{
  public void Handle(CreateReportMessage message)
  {
    Console.WriteLine("Writing Report: {0}", message.Title);
  }
}
```

The message has to be put into the execution engine. To do this an instance of
```IExecutionModule``` has to be configured and obtained. The instance of
```IExecutionModule``´ is put into the target dependency injection container.
This means after the initial configuration it can easily be requested by any custom
object instance which wants to send a message to the lib.

```csharp
public static void Main()
{
  // configures the module to use StructureMap as injection container,
  // otherwise use library defaults.
  IExecutionModule execModule = Module.Configure()
                           .UseStructureMap()
                           .Build();

  // create a custom message and fill it with data.
  var msg = new CreateReportMessage { Title = "Report Title" };

  // add the message into the execution module.
  execModule.Add(msg);

  // message handler will be executed on a separate thread,
  // wait until execution has finished.
  Console.WriteLine("Press any key to quit.");
  Console.ReadKey();
}
```
