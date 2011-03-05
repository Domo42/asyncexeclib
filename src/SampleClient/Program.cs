namespace SampleClient
{
   using System;
   using Domo.AsyncExecutionLib;

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

         // create a module instance with default settings.
         var execModule = Module.Configure()
                           .UseStructureMap()
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

   #region [ Message Handler ]

   /// <summary>
   /// Custom message used in handler
   /// </summary>
   public class MyMessage : IMessage
   {
      /// <summary>
      /// Message text
      /// </summary>
      public string Text { get; set; }
   }

   public class MyMessageHandler : IMessageHandler<MyMessage>
   {
      /// <summary>
      /// Handles the specified message.
      /// </summary>
      /// <param name="message">The message.</param>
      public void Handle(MyMessage message)
      {
         Console.WriteLine();
         Console.WriteLine("***");
         Console.WriteLine("* Custom message has been handled. Text = {0}", message.Text ?? "<null>");
      }
   }

   #endregion
}
