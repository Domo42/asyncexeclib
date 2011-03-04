namespace Domo.ExecutionLib.Execution
{
   using System;

   /// <summary>
   /// Relays incoming message to the execution queue.
   /// </summary>
   public class ExecutionModule : IExecutionModule
   {
      /// <summary>
      /// Add a message to be handled.
      /// </summary>
      /// <param name="message">The message</param>
      public void Add(IMessage message)
      {
      }

      /// <summary>
      /// Action to be executed.
      /// </summary>
      /// <param name="action">The action delegate to execute.</param>
      public void Add(Action action)
      {
      }
   }
}
