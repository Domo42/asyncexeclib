namespace Domo.ExecutionLib.Execution
{
   using System;
   using System.Collections.Generic;

   /// <summary>
   /// Scans and creates message handles based on message type.
   /// </summary>
   public class MessageHandlerCreator : IMessageHandlerCreator
   {
      /// <summary>
      /// Scans for message handler types.
      /// </summary>
      private readonly IMessageHandlerScanner _scanner;

      /// <summary>
      /// Initializes a new instance of the <see cref="MessageHandlerCreator"/> class.
      /// </summary>
      public MessageHandlerCreator(IMessageHandlerScanner scanner)
      {
         _scanner = scanner;
      }

      /// <summary>
      /// Creates a list of handlers associated with the given message.
      /// </summary>
      /// <param name="message">The message to be handled.</param>
      /// <returns>A list of message handlers able to handle the given message.</returns>
      public IEnumerable<IMessageHandler<IMessage>> Create(IMessage message)
      {
         return new IMessageHandler<IMessage>[] { };
      }
   }
}
