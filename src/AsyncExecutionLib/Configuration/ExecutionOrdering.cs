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

namespace OnyxOx.AsyncExecutionLib.Configuration
{
   using System;
   using System.Collections.Generic;

   /// <summary>
   /// Adds message handler types to the prefered order list.
   /// </summary>
   public class ExecutionOrdering
   {
      /// <summary>
      /// Current list of ordered handlers.
      /// </summary>
      private readonly IList<Type> _orderedTypes;

      /// <summary>
      /// Initializes a new instance of the <see cref="ExecutionOrdering"/> class.
      /// </summary>
      /// <param name="orderedTypes">The ordered handlers.</param>
      public ExecutionOrdering(IList<Type> orderedTypes)
      {
         _orderedTypes = orderedTypes;
      }

      /// <summary>
      /// Next message handler in execution chain.
      /// </summary>
      /// <typeparam name="T">Type to be executed in order.</typeparam>
      /// <returns>Executio order instance for further configuration.</returns>
      public ExecutionOrdering Then<T>()
      {
         _orderedTypes.Add(typeof(T));
         return this;
      }
   }
}
