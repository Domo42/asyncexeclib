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
   using System;
   using StructureMap;

   /// <summary>
   /// Builds instances with the help of StructureMap.
   /// </summary>
   public class StructureMapBuilder : IBuilder
   {
      /// <summary>
      /// The structure map container.
      /// </summary>
      private readonly IContainer _container;

      /// <summary>
      /// Initializes a new instance of the <see cref="StructureMapBuilder"/> class.
      /// </summary>
      public StructureMapBuilder() : this(ObjectFactory.Container)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="StructureMapBuilder"/> class.
      /// </summary>
      public StructureMapBuilder(IContainer container)
      {
         _container = container;
      }

      /// <summary>
      /// Returns instance of the requested type.
      /// </summary>
      /// <param name="type">The type of the requested instance.</param>
      /// <returns>Type instance.</returns>
      public object GetInstance(Type type)
      {
         return _container.GetInstance(type);
      }

      /// <summary>
      /// Return instance of requested type.
      /// </summary>
      /// <typeparam name="T">Type of the requested instance.</typeparam>
      /// <returns>Object instance.</returns>
      public T GetInstance<T>()
      {
         return _container.GetInstance<T>();
      }

      /// <summary>
      /// Inform builder of a message handler type to build.
      /// </summary>
      /// <param name="msgHandlerType">Target type</param>
      public void RegisterMsgHandlerType(Type msgHandlerType)
      {
         _container.Configure(x => x.For(msgHandlerType).LifecycleIs(InstanceScope.PerRequest).Use(msgHandlerType));
      }

      /// <summary>
      /// Registers a type which should be configured as singleton.
      /// </summary>
      /// <param name="pluginType">The type for which a singleton instances is to be created.</param>
      /// <param name="concreteType">The concrete type implementing the instance type.</param>
      public void RegisterSingleton(Type pluginType, Type concreteType)
      {
         _container.Configure(x => x.For(pluginType).Singleton().Use(concreteType));
      }

      /// <summary>
      /// Register a type which should be configured to be created on a 'per request' basis.
      /// </summary>
      /// <param name="pluginType">The type for which a singleton instances is to be created.</param>
      /// <param name="concreteType">The concrete type implementing the instance type.</param>
      public void Register(Type pluginType, Type concreteType)
      {
         _container.Configure(x => x.For(pluginType).Use(concreteType));
      }

      /// <summary>
      /// Registers a specific object instance.
      /// </summary>
      /// <typeparam name="T">Type of the object.</typeparam>
      /// <param name="instance">Instance to register.</param>
      public void RegisterInstance<T>(T instance)
      {
         _container.Configure(x => x.For<T>().Use(instance));
      }
   }
}
