
namespace Domo.ExecutionLib.Execution
{
   using System.Collections.Generic;

   /// <summary>
   /// Creates message handlers for specific messages.
   /// </summary>
   public interface IMessageHandlerCreator
   {
      /// <summary>
      /// Creates a list of handlers associated with the given message.
      /// </summary>
      /// <typeparam name="TMessage">Type of the message.</typeparam>
      /// <param name="message">The message to be handled.</param>
      /// <returns>A list of message handlers able to handle the given message.</returns>
      IEnumerable<IMessageHandler<TMessage>> Create<TMessage>(TMessage message) where TMessage : IMessage;
   }
}
