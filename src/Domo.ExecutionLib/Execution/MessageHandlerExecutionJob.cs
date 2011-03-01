namespace Domo.ExecutionLib.Execution
{
   /// <summary>
   /// Executes message handlers for the given message.
   /// </summary>
   /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
   public class MessageHandlerExecutionJob<TMessage> where TMessage : IMessage
   {
      /// <summary>
      /// The message to handle.
      /// </summary>
      private readonly TMessage _message;

      /// <summary>
      /// Creates message handlers for the message.
      /// </summary>
      private readonly IMessageHandlerCreator _handlerCreator;

      /// <summary>
      /// Creates a new <see cref="MessageHandlerExecutionJob&lt;T&gt;"/> instance.
      /// </summary>
      public MessageHandlerExecutionJob(TMessage message, IMessageHandlerCreator handleCreator)
      {
         _message = message;
         _handlerCreator = handleCreator;
      }

      /// <summary>
      /// Execute the job.
      /// </summary>
      public void Execute()
      {
         var handlers = _handlerCreator.Create(_message);
         foreach (var handler in handlers)
         {
            handler.Handle(_message);
         }
      }
   }
}
