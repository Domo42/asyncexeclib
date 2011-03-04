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

namespace Domo.AsyncExecutionLib.Execution
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
