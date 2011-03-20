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
   using System.Diagnostics.CodeAnalysis;
   using System.Reflection;
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
      /// Instance configuration settings.
      /// </summary>
      private readonly Dictionary<Type, IInstanceConfig> _instanceConfigs = new Dictionary<Type, IInstanceConfig>();

      /// <summary>
      /// List of message handler types to be executed before others.
      /// </summary>
      private readonly List<Type> _handlerExecutionOrdering = new List<Type>();

      /// <summary>
      /// List of assemblies used to scan in the the <see cref="Execution.SelectedAssemblyScanner"/>
      /// </summary>
      private readonly List<Assembly> _assembliesToScan = new List<Assembly>();

      /// <summary>
      /// List of actions to execute during module initialization.
      /// </summary>
      private readonly List<Action<IBuilder>> _intializationActions = new List<Action<IBuilder>>();

      /// <summary>
      /// Action to execution before module build is called.
      /// </summary>
      private Action _beforeBuild;

      /// <summary>
      /// Initializes a new instance of the <see cref="ModuleConfig"/> class.
      /// </summary>
      public ModuleConfig()
      {
         // set module defaults
         var scanner = new InstanceConfig<IAssemblyScanner> { ConcreteType = typeof(WorkingDirectoryScanner), IsSingleton = true };
         _instanceConfigs.Add(scanner.PluginType, scanner);

         var pipe = new InstanceConfig<IExecutionPipe> { ConcreteType = typeof(SingleThreadPipe), IsSingleton = true };
         _instanceConfigs.Add(pipe.PluginType, pipe);

         var handleCreator = new InstanceConfig<IMessageHandlerCreator> { ConcreteType = typeof(MessageHandlerCreator), IsSingleton = true };
         _instanceConfigs.Add(handleCreator.PluginType, handleCreator);

         var moduleManager = new InstanceConfig<IModuleManager> { ConcreteType = typeof(ModuleManager), IsSingleton = true };
         _instanceConfigs.Add(moduleManager.PluginType, moduleManager);

         var logger = new InstanceConfig<IAsyncLibLog> { ConcreteType = typeof(DefaultAsyncLibLog), IsSingleton = false };
         _instanceConfigs.Add(logger.PluginType, logger);

         var context = new InstanceConfig<ILocalContext> { ConcreteType = typeof(LocalContext), IsSingleton = true };
         _instanceConfigs.Add(context.PluginType, context);
      }

      /// <summary>
      /// Gets the current instance configuration settings.
      /// </summary>
      internal IDictionary<Type, IInstanceConfig> InstanceConfiguration
      {
         get { return _instanceConfigs; }
      }

      /// <summary>
      /// Adds an action to be executed after configuration, before instances are build.
      /// </summary>
      /// <param name="action">Action to execute.</param>
      internal void AddInitilizationAction(Action<IBuilder> action)
      {
         _intializationActions.Add(action);
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
      public ModuleConfig UseScanner<TScanner>() where TScanner : IAssemblyScanner
      {
         _instanceConfigs[typeof(IAssemblyScanner)].ConcreteType = typeof(TScanner);
         _instanceConfigs[typeof(IAssemblyScanner)].IsSingleton = true;
         return this;
      }

      /// <summary>
      /// Sets a list of specific assemblies to used for type scanning. If ommitted
      /// all assemblies in the current working directory are scanned.
      /// </summary>
      /// <param name="assemblySelector">Select assemblies to scan.</param>
      /// <returns>Module configuration instance.</returns>
      public ModuleConfig ScanSpecificAssemblies(Action<AssemblySelector> assemblySelector)
      {
         UseScanner<SelectedAssemblyScanner>();
         assemblySelector(new AssemblySelector(_assembliesToScan));

         return this;
      }

      /// <summary>
      /// Configures the module to use a specific execution pipe type. If this call
      /// is ommited the <see cref="SingleThreadPipe"/> is used.
      /// </summary>
      /// <typeparam name="TPipe">Type of the execution pipe.</typeparam>
      /// <returns>Module configuration instance.</returns>
      public ModuleConfig UseExecutionPipe<TPipe>() where TPipe : IExecutionPipe
      {
         _instanceConfigs[typeof(IExecutionPipe)].ConcreteType = typeof(TPipe);
         _instanceConfigs[typeof(IExecutionPipe)].IsSingleton = true;
         return this;
      }

      /// <summary>
      /// Instruct the module to create instances of a certain logger type. If not configured
      /// the <see cref="DefaultAsyncLibLog"/> will be used. You should rarely need this. The
      /// recommended way to take care errors is to implement a message module.
      /// </summary>
      /// <typeparam name="TLogger">Type of log instance to created.</typeparam>
      /// <returns>Module configuration instance.</returns>
      public ModuleConfig UseLogger<TLogger>() where TLogger : IAsyncLibLog
      {
         _instanceConfigs[typeof(IAsyncLibLog)].ConcreteType = typeof(TLogger);
         return this;
      }

      /// <summary>
      /// Specifiy a message handler type to be exeucted before the others.
      /// </summary>
      /// <typeparam name="TMEssageHandler">Type of the message handler.</typeparam>
      /// <returns>Handler ordering instruction.</returns>
      public ModuleConfig FirstExecute<TMEssageHandler>()
      {
         _handlerExecutionOrdering.Add(typeof(TMEssageHandler));
         return this;
      }

      /// <summary>
      /// Specifiy a message handler type to be executed before the others. Additional
      /// handlers can be specified calling by calling Next&lt;T&gt;() on the action
      /// argument.
      /// </summary>
      /// <typeparam name="TMessageHandler">Type of the message handler.</typeparam>
      /// <param name="additionalOrdering">Configuration action to specify addtional handlers.</param>
      /// <returns>Module configuration instance.</returns>
      public ModuleConfig FirstExecute<TMessageHandler>(Action<HandlerOrdering> additionalOrdering)
      {
         _handlerExecutionOrdering.Add(typeof(TMessageHandler));

         var nextHandlers = new HandlerOrdering(_handlerExecutionOrdering);
         additionalOrdering(nextHandlers);

         return this;
      }

      /// <summary>
      /// Specify an action to be called before the execution module is constructed
      /// by <see cref="IBuilder"/>. This method can be used to update some last
      /// minute configuration. For example the DI container.
      /// </summary>
      /// <param name="action">Action to execute.</param>
      /// <returns>Module configuration instance.</returns>
      public ModuleConfig BeforeExecutionModuleBuild(Action action)
      {
         _beforeBuild = action;

         return this;
      }

      /// <summary>
      /// Builds a new singleton execution module instance.
      /// </summary>
      /// <returns>The execution module</returns>
      [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UseBuilder", Justification = "Refering to method name.")]
      public IExecutionModule Build()
      {
         if (_builder == null)
         {
            throw new InvalidOperationException(@"No builder has been configured. Call one of the 'UseBuilder' methods before creating the execution module.");
         }

         _builder.RegisterInstance(_builder);

         RegisterExecModule(true);
         RegisterDependencies(true);

         foreach (var action in _intializationActions)
         {
            action(_builder);
         }

         // call action before any actual objects are build.
         if (_beforeBuild != null)
         {
            _beforeBuild();
         }

         SetAssembliesToScan();
         SetPreferedOrder();

         return _builder.GetInstance<IExecutionModule>();
      }

      /// <summary>
      /// Sets message handler ordering.
      /// </summary>
      private void SetPreferedOrder()
      {
         IMessageHandlerCreator handlerCreator = _builder.GetInstance<IMessageHandlerCreator>();
         handlerCreator.SetPreferredOrder(_handlerExecutionOrdering);
      }

      /// <summary>
      /// Applies configuration to the <see cref="SelectedAssemblyScanner"/>
      /// </summary>
      private void SetAssembliesToScan()
      {
         SelectedAssemblyScanner scanner = _builder.GetInstance<SelectedAssemblyScanner>();
         if (scanner != null)
         {
            scanner.SetAssembliesToScan(_assembliesToScan);
         }
      }

      /// <summary>
      /// Register other instance configuration objects.
      /// </summary>
      private void RegisterDependencies(bool singleton)
      {
         foreach (IInstanceConfig instanceConfig in _instanceConfigs.Values)
         {
            if (!instanceConfig.IsSingleton.HasValue)
            {
               instanceConfig.IsSingleton = singleton;
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
   }
}
