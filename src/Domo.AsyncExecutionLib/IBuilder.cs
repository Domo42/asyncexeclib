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
   using System;

   /// <summary>
   /// Builds new object instances.
   /// </summary>
   public interface IBuilder
   {
      /// <summary>
      /// Returns instance of the requested type.
      /// </summary>
      /// <param name="type">The type of the requested instance.</param>
      /// <returns>Object instance.</returns>
      object GetInstance(Type type);

      /// <summary>
      /// Return instance of requested type.
      /// </summary>
      /// <typeparam name="T">Type of the requested instance.</typeparam>
      /// <returns>Object instance.</returns>
      T GetInstance<T>();

      /// <summary>
      /// Inform builder of a message handler type to build.
      /// </summary>
      /// <param name="msgHandlerType">Target type</param>
      void RegisterMsgHandlerType(Type msgHandlerType);

      /// <summary>
      /// Registers a type which should be configured as singleton.
      /// </summary>
      /// <param name="pluginType">The type for which a singleton instances is to be created.</param>
      /// <param name="concreteType">The concrete type implementing the instance type.</param>
      void RegisterSingleton(Type pluginType, Type concreteType);

      /// <summary>
      /// Register a type which should be configured to be created on a 'per request' basis.
      /// </summary>
      /// <param name="pluginType">The type for which a singleton instances is to be created.</param>
      /// <param name="concreteType">The concrete type implementing the instance type.</param>
      void Register(Type pluginType, Type concreteType);

      /// <summary>
      /// Registers a specific object instance.
      /// </summary>
      /// <typeparam name="T">Type of the object.</typeparam>
      /// <param name="instance">Instance to register.</param>
      void RegisterInstance<T>(T instance);
   }
}
