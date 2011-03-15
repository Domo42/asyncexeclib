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

namespace OnyxOx.AsyncExecutionLib
{
   using Configuration;
   using Execution;

   /// <summary>
   /// Used to configure and start the execution module.
   /// </summary>
   public static class Module
   {
      /// <summary>
      /// Start execution module configuration.
      /// </summary>
      /// <returns>Configuration settings.</returns>
      public static ModuleConfig Configure()
      {
         return new ModuleConfig();
      }

      /// <summary>
      /// Configured the module to scan all assemblies in the current working directory
      /// for message handlers.
      /// </summary>
      /// <returns>Module configuration instance.</returns>
      public static ModuleConfig UseCurrentWorkingDirectoryScanner(this ModuleConfig config)
      {
         config.UseScanner<WorkingDirectoryScanner>();
         return config;
      }

      /// <summary>
      /// Configures the execution module with multiple worker threads for execution of
      /// message handlers.
      /// </summary>
      /// <param name="config">Module configuration instance</param>
      /// <param name="numWokerThreads">Number of worker threads to use.</param>
      /// <returns>Module configuration instance</returns>
      public static ModuleConfig UseMultipleWorkerThreads(this ModuleConfig config, int numWokerThreads)
      {
         config.InstanceConfiguration[typeof(IExecutionPipe)].ConcreteType = typeof(MultiThreadPipe);
         config.InstanceConfiguration[typeof(IExecutionPipe)].IsSingleton = true;

         config.AddInitilizationAction(builder =>
               {
                  var execPipe = builder.GetInstance<IExecutionPipe>() as MultiThreadPipe;
                  if (execPipe != null)
                  {
                     // one thread already present in exec pipe
                     int threadsToAdd = numWokerThreads - 1;
                     execPipe.IncreaseWorkerThreads(threadsToAdd);
                  }
               });

         return config;
      }
   }
}
