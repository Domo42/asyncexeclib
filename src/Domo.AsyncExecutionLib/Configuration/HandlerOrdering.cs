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

namespace Domo.AsyncExecutionLib.Configuration
{
   using System;
   using System.Collections.Generic;

   /// <summary>
   /// Adds message handler types to the prefered order list.
   /// </summary>
   public class HandlerOrdering
   {
      /// <summary>
      /// Current list of ordered handlers.
      /// </summary>
      private readonly IList<Type> _orderedHandlers;

      /// <summary>
      /// Initializes a new instance of the <see cref="HandlerOrdering"/> class.
      /// </summary>
      /// <param name="orderedHandlers">The ordered handlers.</param>
      public HandlerOrdering(IList<Type> orderedHandlers)
      {
         _orderedHandlers = orderedHandlers;
      }

      /// <summary>
      /// Next message handler in execution chain.
      /// </summary>
      /// <typeparam name="TMessageHandler">Type of the message handler</typeparam>
      /// <returns></returns>
      public HandlerOrdering Then<TMessageHandler>()
      {
         _orderedHandlers.Add(typeof(TMessageHandler));
         return this;
      }
   }
}
