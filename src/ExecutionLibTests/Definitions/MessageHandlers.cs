namespace ExecutionLibTests
{
   using System;
   using Domo.ExecutionLib;

   public class MessageHandlerBase
   {
      public event EventHandler<MessageHandlerCalledEventArgs> HandlerCalled;

      protected virtual void OnHandlerCalled()
      {
         var handler = this.HandlerCalled;
         if (handler != null)
         {
            handler(this, new MessageHandlerCalledEventArgs(this.GetType()));
         }
      }
   }

   public class InterfaceMessageHandler : MessageHandlerBase, IMessageHandler<IMessage>
   {
      /// <summary>
      /// Handles the specified message.
      /// </summary>
      /// <param name="message">The message.</param>
      public void Handle(IMessage message)
      {
         OnHandlerCalled();
      }
   }

   public class MessageBaseHandler : MessageHandlerBase, IMessageHandler<MessageBase>
   {
      public void Handle(MessageBase message)
      {
         OnHandlerCalled();
      }
   }

   public class SeparateMessgeHandler : MessageHandlerBase, IMessageHandler<SeparateMessage>
   {
      public void Handle(SeparateMessage message)
      {
         OnHandlerCalled();
      }
   }

   public class MessageHandlerCalledEventArgs : EventArgs
   {
      public MessageHandlerCalledEventArgs(Type handlerType)
      {
         this.HandlerType = handlerType;
      }
      
      public Type HandlerType { get; private set; }
   }
}
