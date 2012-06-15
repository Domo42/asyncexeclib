#region [ File Header ]
/*****************************************************************************
  Copyright 2011 Stefan Domnanovits

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at
 
      http://www.apache.org/licenses/LICENSE-2.0
 
  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
/****************************************************************************/
#endregion

namespace ExecutionLibTests
{
   using System;
   using OnyxOx.AsyncExecutionLib;

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

   public class InterfaceMessageHandler : MessageHandlerBase, IMessageHandler<object>
   {
      /// <summary>
      /// Handles the specified message.
      /// </summary>
      /// <param name="message">The message.</param>
      public void Handle(object message)
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
