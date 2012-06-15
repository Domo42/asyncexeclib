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
   /// Execution context local to the current execution thread
   /// and message. Content is cleared once message processing has finished
   /// and thread is used for a new message.
   /// </summary>
   public interface ILocalContext
   {
      /// <summary>
      /// Gets the currently processed message.
      /// </summary>
      object Message { get; }

      /// <summary>
      /// Gets or sets a value indicating whether to stop executing further handler
      /// of the currently handled message.
      /// </summary>
      bool DoNotContinueDispatchingCurrentMessageToHandlers { get; set; }

      /// <summary>
      /// Gets or sets a local context variable to the context.
      /// </summary>
      /// <param name="key">Key of the local variable.</param>
      /// <returns>Value associated with the key.</returns>
      object this[object key] { get; set; }

      /// <summary>
      /// Gets a local context variable of a specific type.
      /// </summary>
      /// <typeparam name="TEntry">Type of the variable.</typeparam>
      /// <param name="key">Key of the local variable.</param>
      /// <returns>Value associated with the key.</returns>
      TEntry Get<TEntry>(object key);

      /// <summary>
      /// Sets a local context variable.
      /// </summary>
      /// <param name="key">Key of the local variable.</param>
      /// <param name="value">Value associated with the key.</param>
      void Set(object key, object value);

      /// <summary>
      /// Gets a local context variable of a specifiy type.
      /// </summary>
      /// <typeparam name="TEntry">Type of the variable.</typeparam>
      /// <param name="key">Key of the local variable.</param>
      /// <param name="entry">If successfull, entry contains the local variable value.</param>
      /// <returns>True if value retrieved succesfully; otherwise false.</returns>
      bool TryGetValue<TEntry>(object key, out TEntry entry);

      /// <summary>
      /// Indicates whether a local context variable with the given key exists.
      /// </summary>
      /// <param name="key">Key of the local variable.</param>
      /// <returns>True if variable exists.</returns>
      bool Contains(object key);
   }
}
