﻿#region [ File Header ]
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
   using System.Collections.Concurrent;
   using System.Collections.Generic;
   using System.Linq;

   /// <summary>
   /// Scans and creates message handles based on message type.
   /// </summary>
   public class MessageHandlerCreator : IMessageHandlerCreator
   {
      /// <summary>
      /// Scans for message handler types.
      /// </summary>
      private readonly IAssemblyScanner _scanner;

      /// <summary>
      /// Builds handler instances.
      /// </summary>
      private readonly IBuilder _builder;

      /// <summary>
      /// List of available handlers, ordered by message type.
      /// </summary>
      private readonly Dictionary<Type, List<Type>> _scannedHandlers = new Dictionary<Type, List<Type>>();

      /// <summary>
      /// Lisft of handlers, including base type handlers ordered by incoming msg type.
      /// </summary>
      private readonly ConcurrentDictionary<Type, List<Type>> _handlers = new ConcurrentDictionary<Type,List<Type>>();

      /// <summary>
      /// Prefered message handler ordering.
      /// </summary>
      private IEnumerable<Type> _preferedOrder = new List<Type>();

      /// <summary>
      /// Initializes a new instance of the <see cref="MessageHandlerCreator"/> class.
      /// </summary>
      public MessageHandlerCreator(IAssemblyScanner scanner, IBuilder builder)
      {
         _scanner = scanner;
         _builder = builder;
         GroupHandlersByHandledTypes();
      }

      /// <summary>
      /// Creates a list of handlers associated with the given message.
      /// </summary>
      /// <param name="message">The message to be handled.</param>
      /// <returns>A list of message handlers able to handle the given message.</returns>
      public IEnumerable<IMessageHandler<TMessage>> Create<TMessage>(TMessage message) where TMessage : class
      {
         IEnumerable<IMessageHandler<TMessage>> handlerInstances;

         if (message != null)
         {
            Type msgType = message.GetType();
            List<Type> handlerTypes = _handlers.GetOrAdd(msgType, GetAllHandlersForType);

            handlerInstances = handlerTypes.Select(x => (IMessageHandler<TMessage>)_builder.GetInstance(x));
         }
         else
         {
            handlerInstances = new IMessageHandler<TMessage>[] { };
         }

         return handlerInstances;
      }

      /// <summary>
      /// Sets a a preferred execution order for message handler types.
      /// </summary>
      /// <param name="handlers">List of message handler types in expected order.</param>
      public void SetPreferredOrder(IEnumerable<Type> handlers)
      {
         if (handlers != null)
         {
            _preferedOrder = handlers;
         }
         else
         {
            _preferedOrder = new List<Type>();
         }
      }

      /// <summary>
      /// Creates a list of handler types, based on the msg types and its
      /// base types and interfaces.
      /// </summary>
      /// <returns>List of message handler types.</returns>
      private List<Type> GetAllHandlersForType(Type msgType)
      {
         var handlerTypes = new List<Type>();

         // Iterate through class structure
         Type targetType = msgType;
         while (targetType != null)
         {
            List<Type> msgHandlers;
            if (_scannedHandlers.TryGetValue(targetType, out msgHandlers))
            {
               handlerTypes.AddRange(msgHandlers);
            }

            targetType = targetType.BaseType;
         }

         // check for msg interface handlers (ex. IResponse, ...)
         var interfaces = msgType.GetInterfaces();
         foreach (Type interfaceType in interfaces)
         {
            List<Type> msgHandlers;
            if (_scannedHandlers.TryGetValue(interfaceType, out msgHandlers))
            {
               handlerTypes.AddRange(msgHandlers);
            }
         }

         foreach (Type handlerType in handlerTypes)
         {
            _builder.RegisterMsgHandlerType(handlerType);
         }

         return CreatePreferredExecutionOrder(handlerTypes);
      }

      /// <summary>
      /// If set, puts preferred handlers in front.
      /// </summary>
      /// <returns>Sorted collection.</returns>
      private List<Type> CreatePreferredExecutionOrder(List<Type> unsortedHandlers)
      {
         List<Type> sorted = new List<Type>();

         // Search for preferred types and put them in front.
         foreach (Type sortedType in _preferedOrder.Reverse())
         {
            foreach (Type unsortedType in unsortedHandlers)
            {
               if (sortedType == unsortedType)
               {
                  if (!sorted.Contains(unsortedType))
                  {
                     // put in front if preferred to be executed before
                     // unspecified ones.
                     sorted.Insert(0, unsortedType);
                  }
               }
            }
         }

         // append all handlers not already in list
         foreach (Type handler in unsortedHandlers)
         {
            if (!sorted.Contains(handler))
            {
               sorted.Add(handler);
            }
         }

         return sorted;
      }

      /// <summary>
      /// Searches for all handlers and creates a handler chain called if a specific message is
      /// to be handled.
      /// </summary>
      private void GroupHandlersByHandledTypes()
      {
         IEnumerable<Type> messageHandlerTypes = _scanner.ScanForMessageHandlers();
         if (messageHandlerTypes != null)
         {
            foreach (Type msgHandler in messageHandlerTypes)
            {
               var handlerInterfaceTypes = msgHandler.GetInterfaces().Where(IsMessageHandlerInterface);
               foreach (Type handlerInterface in handlerInterfaceTypes)
               {
                  Type handledMsgType = handlerInterface.GetGenericArguments().First();

                  AddToScannedHandlers(handledMsgType, msgHandler);
               }
            }
         }
      }

      /// <summary>
      /// Adds to handlers execution dictionary. Creates new entry if entry
      /// for message type does not yet exists.
      /// </summary>
      private void AddToScannedHandlers(Type handledMsgType, Type msgHandler)
      {
         List<Type> handlerTypes;
         if (!_scannedHandlers.TryGetValue(handledMsgType, out handlerTypes))
         {
            handlerTypes = new List<Type>();
            _scannedHandlers.Add(handledMsgType, handlerTypes);
         }

         handlerTypes.Add(msgHandler);
      }

      /// <summary>
      /// Returns true if type implements at least one message handler.
      /// </summary>
      /// <returns>True if implemented.</returns>
      private static bool IsMessageHandlerInterface(Type interfaceType)
      {
         return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IMessageHandler<>);
      }
   }
}
