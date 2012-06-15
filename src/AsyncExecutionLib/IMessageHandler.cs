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

namespace OnyxOx.AsyncExecutionLib
{
   /// <summary>
   /// Handles custom messages.
   /// </summary>
   /// <typeparam name="TMessage">Type of message to handle.</typeparam>
   public interface IMessageHandler<in TMessage> where TMessage : class
   {
      /// <summary>
      /// Handles the message.
      /// </summary>
      /// <param name="message">The message to handle.</param>
      void Handle(TMessage message);
   }
}
