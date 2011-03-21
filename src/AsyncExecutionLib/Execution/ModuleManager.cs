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
      /// Scanner to search for modules.
      /// </summary>
      private readonly IAssemblyScanner _scanner;

      /// <summary>
      /// Builder to create message module instances.
      /// </summary>
      private readonly IBuilder _builder;

      /// <summary>
      /// List of stored message modules.
      /// </summary>
      private List<IMessageModule> _modules;

      /// <summary>
      /// Prefered execution order.
      /// </summary>
      private IEnumerable<Type> _preferredExecutionOrder = new List<Type>();

      /// <summary>
      /// Initializes a new instance of the <see cref="ModuleManager"/> class.
      /// </summary>
      public ModuleManager(IAssemblyScanner scanner, IBuilder builder)
      {
         _scanner = scanner;
         _builder = builder;
      }

      /// <summary>
      /// Will be called before any message handlers are executed.
      /// </summary>
      public void OnStart()
      {
         CreateModuleInstances();

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
      /// Sets a a prefered execution order for message module instances.
      /// </summary>
      /// <param name="modules">List of message module types in expected order.</param>
      public void SetPreferredOrder(IEnumerable<Type> modules)
      {
         if (modules != null)
         {
            _preferredExecutionOrder = modules;
         }
      }

      /// <summary>
      /// Create the scanned types message modules instances.
      /// </summary>
      private void CreateModuleInstances()
      {
         if (_modules == null)
         {
            _modules = new List<IMessageModule>();

            IEnumerable<Type> moduleTypes = _scanner.ScanForMessageModules();
            if (moduleTypes != null)
            {
               BuildModuleListAccordingToPreference(moduleTypes);
            }
         }
      }

      /// <summary>
      /// Creates the modules instance list in preferred execution order.
      /// </summary>
      private void BuildModuleListAccordingToPreference(IEnumerable<Type> scannedModules)
      {
         List<Type> moduleTypes = new List<Type>();
         
         // first add preferred types.
         foreach (Type moduleType in _preferredExecutionOrder)
         {
            if (scannedModules.Contains(moduleType)
               && !moduleTypes.Contains(moduleType))
            {
               moduleTypes.Add(moduleType);
            }
         }

         // add other scanned types
         foreach (Type moduleType in scannedModules)
         {
            if (!moduleTypes.Contains(moduleType))
            {
               moduleTypes.Add(moduleType);
            }
         }

         // create message module instances
         foreach (Type moduleType in moduleTypes)
         {
            _builder.Register(moduleType, moduleType);

            IMessageModule module = (IMessageModule)_builder.GetInstance(moduleType);
            _modules.Add(module);
         }
      }
   }
}
