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
   using System.Diagnostics.CodeAnalysis;
   using Execution;

   /// <summary>
   /// Holds the module configuration until execution module has
   /// been started.
   /// </summary>
   public class ModuleConfig
   {
      /// <summary>
      /// Builder to create lib object instances.
      /// </summary>
      private IBuilder _builder;

      /// <summary>
      /// The configured handler scanner.
      /// </summary>
      private readonly InstanceConfig<IMessageHandlerScanner> _scanner = new InstanceConfig<IMessageHandlerScanner>();

      /// <summary>
      /// The execution queue to be used.
      /// </summary>
      private readonly InstanceConfig<IExecutionQueue> _execQueue = new InstanceConfig<IExecutionQueue>();

      /// <summary>
      /// Initializes a new instance of the <see cref="ModuleConfig"/> class.
      /// </summary>
      public ModuleConfig()
      {
         // set module defaults
         _scanner.ConcreteType = typeof(WorkingDirectoryScanner);
         _execQueue.ConcreteType = typeof(SingleThreadQueue);
      }

      /// <summary>
      /// Defines a builder to be used to create object instances inside the
      /// execution module.
      /// </summary>
      /// <returns>Module configuration instance.</returns>
      public ModuleConfig UseBuilder(IBuilder builder)
      {
         _builder = builder;
         return this;
      }

      /// <summary>
      /// Custom message scanner to use. If ommited all assemblies in the current working directory
      /// are scanned for message handlers.
      /// </summary>
      /// <typeparam name="TScanner">Concrete message handler scanner type.</typeparam>
      /// <returns>Module configuration instance.</returns>
      public ModuleConfig UseScanner<TScanner>() where TScanner : IMessageHandlerScanner
      {
         UseScanner<TScanner>(true);
         return this;
      }

      /// <summary>
      /// Custom message scanner to use. If ommited all assemblies in the current working directory
      /// are scanned for message handlers.
      /// </summary>
      /// <typeparam name="TScanner">Concrete message handler scanner type.</typeparam>
      /// <param name="singleton">Indicates whether to register the scanner as a singleton.</param>
      /// <returns>Module configuration instance.</returns>
      public ModuleConfig UseScanner<TScanner>(bool singleton) where TScanner : IMessageHandlerScanner
      {
         _scanner.ConcreteType = typeof(TScanner);
         _scanner.IsSingleton = singleton;
         return this;
      }

      /// <summary>
      /// Configured the module to scan all assemblies in the current working directory
      /// for message handlers.
      /// </summary>
      /// <returns>Module configuration instance.</returns>
      public ModuleConfig UseCurrentWorkingDirectoryScanner()
      {
         _scanner.ConcreteType = typeof(WorkingDirectoryScanner);
         return this;
      }

      /// <summary>
      /// Builds a new singleton execution module instance.
      /// </summary>
      /// <returns>The execution module</returns>
      public IExecutionModule Build()
      {
         return Build(true);
      }

      /// <summary>
      /// Builds a new execution module instance.
      /// </summary>
      /// <param name="singleton">Indicates whether the execution module should be created as singleton.</param>
      /// <returns>The execution module</returns>
      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UseBuilder")]
      public IExecutionModule Build(bool singleton)
      {
         if (_builder == null)
         {
            throw new InvalidOperationException(@"No builder has been configured. Call one of the 'UseBuilder' methods before creating the execution module.");
         }

         RegisterExecModule(singleton);
         RegisterInstance(singleton, _scanner);
         RegisterInstance(singleton, _execQueue);

         return _builder.GetInstance<IExecutionModule>();
      }

      /// <summary>
      /// Registers the execution module with the builder.
      /// </summary>
      private void RegisterExecModule(bool singleton)
      {
         if (singleton)
         {
            _builder.RegisterSingleton(typeof(IExecutionModule), typeof(ExecutionModule));
         }
         else
         {
            _builder.Register(typeof(IExecutionModule), typeof(ExecutionModule));
         }
      }

      /// <summary>
      /// Registers the given instance in the builder.
      /// </summary>
      /// <typeparam name="TPluginType">Type to register.</typeparam>
      private void RegisterInstance<TPluginType>(bool isModuleSingleton, InstanceConfig<TPluginType> instanceConfig)
      {
         if (!instanceConfig.IsSingleton.HasValue)
         {
            instanceConfig.IsSingleton = isModuleSingleton;
         }

         if (instanceConfig.IsSingleton.Value)
         {
            _builder.RegisterSingleton(instanceConfig.PluginType, instanceConfig.ConcreteType);
         }
         else
         {
            _builder.Register(instanceConfig.PluginType, instanceConfig.ConcreteType);
         }
      }
   }
}
