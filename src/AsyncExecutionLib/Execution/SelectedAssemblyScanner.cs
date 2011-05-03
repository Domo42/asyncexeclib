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
   using System.Reflection;

   /// <summary>
   /// Scanns for message handlers and modules only in specific assemblies.
   /// </summary>
   public class SelectedAssemblyScanner : AssemblyScanner, IAssemblyScanner
   {
      /// <summary>
      /// List of assemblies to be used for scanning.
      /// </summary>
      private IEnumerable<Assembly> _assemblies = new List<Assembly>();

      /// <summary>
      /// Initializes a new instance of the <see cref="SelectedAssemblyScanner"/> class.
      /// </summary>
      public SelectedAssemblyScanner()
      {   
      }

      /// <summary>
      /// Sets a list to scan for assemblies.
      /// </summary>
      public void SetAssembliesToScan(IEnumerable<Assembly> assemblies)
      {
         _assemblies = assemblies ?? new List<Assembly>();
      }

      /// <summary>
      /// Scan for message handlers.
      /// </summary>
      /// <returns>List of available handlers.</returns>
      public IEnumerable<Type> ScanForMessageHandlers()
      {
         List<Type> messageHandlerTypes = new List<Type>();
         foreach (Assembly assembly in _assemblies)
         {
            messageHandlerTypes.AddRange(ScanAssembly(assembly, IsMessageHandlerInterface));
         }

         return messageHandlerTypes;
      }

      /// <summary>
      /// Scan for message module types.
      /// </summary>
      /// <returns>List of available message modules.</returns>
      public IEnumerable<Type> ScanForMessageModules()
      {
         List<Type> moduleTypes = new List<Type>();
         foreach (Assembly assembly in _assemblies)
         {
            moduleTypes.AddRange(ScanAssembly(assembly, IsMessageModuleInterface));
         }

         return moduleTypes;
      }
   }
}
