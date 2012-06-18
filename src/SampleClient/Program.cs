namespace SampleClient
{
   using System;
   using OnyxOx.AsyncExecutionLib;

   /// <summary>
   /// Main program entry class.
   /// </summary>
   class Program
   {
      /// <summary>
      /// Main program entry point.
      /// </summary>
      static void Main()
      {
         // give us some space.
         Console.WriteLine();

         // initialize the Autofac DI container.
         var afBuilder = new Autofac.ContainerBuilder();
         using (var container = afBuilder.Build())
         {
            // create a module instance with default settings.
            var execModule = Module.Configure()
                  .UseAutofac(container)
                  .FirstExecute<LogMessageHandler>()
                  .Build();

            // create a new message to be handled.
            var msg = new MyMessage { Text = "Hello message handler!" };

            // add the message to the execution module.
            execModule.Add(msg);

            // message handler will be exeucted on a separate thread,
            // therefore we wait until satisfied.
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
         }
      }
   }

   #region [ Message Handler ]

   /// <summary>
   /// Custom message used in handler
   /// </summary>
   public class MyMessage
   {
      /// <summary>
      /// Message text
      /// </summary>
      public string Text { get; set; }
   }

   /// <summary>
   /// Message handler of the concrete message.
   /// </summary>
   public class MyMessageHandler : IMessageHandler<MyMessage>
   {
      /// <summary>
      /// Handles the specified message.
      /// </summary>
      /// <param name="message">The message.</param>
      public void Handle(MyMessage message)
      {
         Console.WriteLine("MyMessageHandler : Custom message has been handled. Text = {0}", message.Text ?? "<null>");
      }
   }

   /// <summary>
   /// Message handler matching any base IMessage.
   /// </summary>
   public class LogMessageHandler : IMessageHandler<object>
   {
      public void Handle(object message)
      {
         Console.WriteLine();
         Console.WriteLine("LogMessageHandler: Message of type '{0}' received.", message.GetType().Name);
      }
   }

   #endregion
}
