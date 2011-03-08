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
   using System.IO;
   using System.Linq;
   using System.Reflection;
   using log4net;

   /// <summary>
   /// Scans all assemblies of the current working directory for
   /// Message handlers.
   /// </summary>
   public class WorkingDirectoryScanner : IAssemblyScanner
   {
      /// <summary>
      /// Scan for message handlers.
      /// </summary>
      /// <returns>List of available handlers.</returns>
      public IEnumerable<Type> ScanForMessageHandlers()
      {
         return ScanWorkingDirctory(IsMessageHandlerInterface);
      }

      /// <summary>
      /// Scan for message module types.
      /// </summary>
      /// <returns>List of available message modules.</returns>
      public IEnumerable<Type> ScanForMessageModules()
      {
         return ScanWorkingDirctory(IsMessageModuleInterface);
      }

      /// <summary>
      /// Scan the working directory for types matching the predicate.
      /// </summary>
      private IEnumerable<Type> ScanWorkingDirctory(Predicate<Type> targetType)
      {
         List<Type> handlerTypes = new List<Type>();

         string currentDir = Environment.CurrentDirectory;
         foreach (string file in Directory.GetFiles(currentDir, "*.dll"))
         {
            handlerTypes.AddRange(ScanPossibleAssembly(file, targetType));
         }

         foreach (string file in Directory.GetFiles(currentDir, "*.exe"))
         {
            handlerTypes.AddRange(ScanPossibleAssembly(file, targetType));
         }

         return handlerTypes;
      }

      /// <summary>
      /// Scan the file, which may or may not be an actual assembly.
      /// </summary>
      private IEnumerable<Type> ScanPossibleAssembly(string fileName, Predicate<Type> targeType)
      {
         IEnumerable<Type> handlers = null;

         try
         {
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(fileName);
            Assembly assembly = Assembly.Load(assemblyName);
            handlers = ScanAssembly(assembly, targeType);
         }
         catch (BadImageFormatException)
         {
            // file is not a .net assembly.
         }

         return handlers ?? new Type[0];
      }

      /// <summary>
      /// Scans an assembly for message handler implementations.
      /// </summary>
      private IEnumerable<Type> ScanAssembly(Assembly assembly, Predicate<Type> targetType)
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
      /// Returns true if interface is of type IMessageModule.
      /// </summary>
      /// <returns>True if </returns>
      private bool IsMessageModuleInterface(Type interfaceType)
      {
         return interfaceType.IsInterface && typeof(IMessageModule).IsAssignableFrom(interfaceType);
      }

      /// <summary>
      /// Returns true if type implements at least one message handler.
      /// </summary>
      /// <returns>True if implemented.</returns>
      private bool IsMessageHandlerInterface(Type interfaceType)
      {
         return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IMessageHandler<>);
      }
   }
}
