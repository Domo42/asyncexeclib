namespace Domo.AsyncExecutionLib
{
   /// <summary>
   /// Handles custom messages.
   /// </summary>
   /// <typeparam name="TMessage">Type of message to handle.</typeparam>
   public interface IMessageHandler<in TMessage> where TMessage : IMessage
   {
      /// <summary>
      /// Handles the message.
      /// </summary>
      /// <param name="message">The message to handle.</param>
      void Handle(TMessage message);
   }
}
