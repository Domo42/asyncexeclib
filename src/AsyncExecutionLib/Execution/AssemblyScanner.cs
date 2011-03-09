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
   using System.Linq;
   using System.Reflection;

   /// <summary>
   /// Base assembly scanning functionality. An examples of concrete implementations
   /// is <see cref="WorkingDirectoryScanner"/>
   /// </summary>
   public abstract class AssemblyScanner
   {
      /// <summary>
      /// Scans an assembly for message handler implementations.
      /// </summary>
      protected IEnumerable<Type> ScanAssembly(Assembly assembly, Predicate<Type> targetType)
      {
         List<Type> handlerTypes = new List<Type>();

         foreach (Type t in assembly.GetTypes())
         {
            if (!t.IsAbstract)
            {
               Type[] interfaces = t.GetInterfaces();
               if (interfaces.Any(i => targetType(i)))
               {
                  handlerTypes.Add(t);
               }
            }
         }

         return handlerTypes;
      }

      /// <summary>
      /// Predicate returning true if interface is of type IMessageModule.
      /// </summary>
      /// <returns>True if IMessageModule interface</returns>
      protected bool IsMessageModuleInterface(Type interfaceType)
      {
         return interfaceType.IsInterface && typeof(IMessageModule).IsAssignableFrom(interfaceType);
      }

      /// <summary>
      /// Prdicate returning true if type implements at least one message handler.
      /// </summary>
      /// <returns>True if implemented.</returns>
      protected bool IsMessageHandlerInterface(Type interfaceType)
      {
         return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IMessageHandler<>);
      }
   }
}
