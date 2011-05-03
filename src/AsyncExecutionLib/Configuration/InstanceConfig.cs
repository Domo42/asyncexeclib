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

   /// <summary>
   /// Holds the configuration for a single instance to be created.
   /// </summary>
   internal class InstanceConfig<TPluginType> : IInstanceConfig
   {
      /// <summary>
      /// Gets the type to be created.
      /// </summary>
      public Type PluginType
      {
         get { return typeof(TPluginType); }
      }

      /// <summary>
      /// Gets or sets the concrety type of the instance to be created.
      /// </summary>
      public Type ConcreteType { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether the instance should be created
      /// as a singleton.
      /// </summary>
      public bool? IsSingleton { get; set; }
   }
}
