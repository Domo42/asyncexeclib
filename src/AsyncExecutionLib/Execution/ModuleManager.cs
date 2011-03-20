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

   /// <summary>
   /// Creates the messagem module instances and calls the modules
   /// callback methods.
   /// </summary>
   public class ModuleManager : IModuleManager
   {
      /// <summary>
      /// List of stored message modules.
      /// </summary>
      private readonly List<IMessageModule> _modules = new List<IMessageModule>();

      /// <summary>
      /// Initializes a new instance of the <see cref="ModuleManager"/> class.
      /// </summary>
      public ModuleManager(IAssemblyScanner scanner, IBuilder builder)
      {
         CreateModuleInstances(scanner, builder);
      }

      /// <summary>
      /// Will be called before any message handlers are executed.
      /// </summary>
      public void OnStart()
      {
         foreach (IMessageModule module in _modules)
         {
            module.OnStart();
         }
      }

      /// <summary>
      /// Will be called after message handlers have executed. Will be called
      /// even in case an error has occurred.
      /// </summary>
      public void OnFinished()
      {
         foreach (IMessageModule module in _modules.Reverse<IMessageModule>())
         {
            module.OnFinished();
         }
      }

      /// <summary>
      /// Will be called in case one of the message handlers throws an exception.
      /// If an error occurrs this method will be called before <see cref="IModuleManager.OnFinished"/>.
      /// </summary>
      /// <param name="ex">The exception thrown.</param>
      public void OnError(Exception ex)
      {
         foreach (IMessageModule module in _modules.Reverse<IMessageModule>())
         {
            module.OnError(ex);
         }
      }

      /// <summary>
      /// Create the scanned types message modules instances.
      /// </summary>
      private void CreateModuleInstances(IAssemblyScanner scanner, IBuilder builder)
      {
         IEnumerable<Type> moduleTypes = scanner.ScanForMessageModules();
         if (moduleTypes != null)
         {
            foreach (Type moduleType in moduleTypes)
            {
               builder.Register(moduleType, moduleType);

               IMessageModule module = (IMessageModule) builder.GetInstance(moduleType);
               _modules.Add(module);
            }
         }
      }
   }
}
