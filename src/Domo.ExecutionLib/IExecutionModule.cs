namespace Domo.ExecutionLib
{
   using System;

   /// <summary>
   /// Executes messages handler of added messages.
   /// </summary>
   public interface IExecutionModule
   {
      /// <summary>
      /// Add a message to be handled.
      /// </summary>
      /// <param name="message">The message</param>
      void Add(IMessage message);

      /// <summary>
      /// Action to be executed.
      /// </summary>
      /// <param name="action">The action delegate to execute.</param>
      void Add(Action action);
   }
}
