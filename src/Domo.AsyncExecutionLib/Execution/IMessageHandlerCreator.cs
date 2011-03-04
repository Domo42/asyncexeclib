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
