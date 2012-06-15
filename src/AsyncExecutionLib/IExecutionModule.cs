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
   using System;

   /// <summary>
   /// Executes messages handler of added messages.
   /// </summary>
   public interface IExecutionModule
   {
      /// <summary>
      /// Add a message to be handled.
      /// </summary>
      /// <param name="message">The message to handle.</param>
      void Add<T>(T message) where T : class;

      /// <summary>
      /// Action to be executed.
      /// </summary>
      /// <param name="action">The action delegate to execute.</param>
      void Add(Action action);

      /// <summary>
      /// Handle the message in a synchron operation. This will not put
      /// the message into the configured execution pipe.
      /// </summary>
      /// <param name="message">The message to handle.</param>
      void Handle<T>(T message) where T : class;
   }
}
