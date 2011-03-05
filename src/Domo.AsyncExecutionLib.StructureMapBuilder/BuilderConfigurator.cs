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

namespace Domo.AsyncExecutionLib
{
   using Configuration;
   using StructureMap;

   /// <summary>
   /// Adds extension methods to the async execution lib module
   /// configuration.
   /// </summary>
   public static class BuilderConfigurator
   {
      /// <summary>
      /// Configures the execution module to use the StructureMap default 
      /// container to build object instances.
      /// </summary>
      public static ModuleConfig UseStructureMap(this ModuleConfig config)
      {
         UseStructureMap(config, ObjectFactory.Container);
         return config;
      }

      /// <summary>
      /// Configures the execution module to use the StructureMap container
      /// to build object instances.
      /// </summary>
      /// <param name="config">Module configuration.</param>
      /// <param name="container">The specific container to use.</param>
      /// <returns>Module configuration.</returns>
      public static ModuleConfig UseStructureMap(this ModuleConfig config, IContainer container)
      {
         config.UseBuilder(new StructureMapBuilder(container));
         return config;
      }
   }
}
