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

namespace OnyxOx.AsyncExecutionLib.Execution
{
   using System;
   using System.Collections.Generic;

   /// <summary>
   /// Stores execution context in thread local variables.
   /// </summary>
   public class LocalContext : ILocalContext
   {
      /// <summary>
      /// The message to which this context is assigned to.
      /// </summary>
      [ThreadStatic]
      private static IMessage _message;

      /// <summary>
      /// Holds a list of thread local entries.
      /// </summary>
      [ThreadStatic]
      private static Dictionary<object, object> _contextVariables;

      /// <summary>
      /// Indicates whether to stop executing further handler of this message.
      /// </summary>
      [ThreadStatic]
      private static bool _stopHandlingMessages;

      /// <summary>
      /// Initializes the current execution context with the given message.
      /// Clears any previous data.
      /// </summary>
      public void Initialize(IMessage message)
      {
         _stopHandlingMessages = false;
         _message = message;

         if (_contextVariables == null)
         {
            _contextVariables = new Dictionary<object, object>();
         }

         _contextVariables.Clear();
      }

      /// <summary>
      /// Gets the currently processed message.
      /// </summary>
      public IMessage Message
      {
         get { return _message; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether to stop executing further handler
      /// of the currently handled message.
      /// </summary>
      public bool DoNotContinueDispatchingCurrentMessageToHandlers
      {
         get { return _stopHandlingMessages; }
         set { _stopHandlingMessages = value; }
      }

      /// <summary>
      /// Gets or sets the <see cref="System.Object"/> with the specified key.
      /// </summary>
      public object this[object key]
      {
         get
         {
            return _contextVariables[key];
         }

         set
         {
            _contextVariables[key] = value;
         }
      }

      /// <summary>
      /// Gets a local context variable of a specific type.
      /// </summary>
      /// <typeparam name="TEntry">Type of the variable.</typeparam>
      /// <param name="key">Key of the local variable.</param>
      /// <returns>Value associated with the key.</returns>
      public TEntry Get<TEntry>(object key)
      {
         return (TEntry)_contextVariables[key];
      }

      /// <summary>
      /// Sets a local context variable.
      /// </summary>
      /// <param name="key">Key of the local variable.</param>
      /// <param name="value">Value associated with the key.</param>
      public void Set(object key, object value)
      {
         this[key] = value;
      }

      /// <summary>
      /// Gets a local context variable of a specifiy type.
      /// </summary>
      /// <typeparam name="TEntry">Type of the variable.</typeparam>
      /// <param name="key">Key of the local variable.</param>
      /// <param name="entry">If successfull, entry contains the local variable value.</param>
      /// <returns>True if value retrieved succesfully; otherwise false.</returns>
      public bool TryGetValue<TEntry>(object key, out TEntry entry)
      {
         bool isSuccess = false;
         entry = default(TEntry);

         object variable;
         if (_contextVariables.TryGetValue(key, out variable))
         {
            if (variable is TEntry)
            {
               entry = (TEntry)variable;
               isSuccess = true;
            }
         }

         return isSuccess;
      }

      /// <summary>
      /// Indicates whether a local context variable with the given key exists.
      /// </summary>
      /// <param name="key">Key of the local variable.</param>
      /// <returns>True if variable exists.</returns>
      public bool Contains(object key)
      {
         return _contextVariables.ContainsKey(key);
      }
   }
}
