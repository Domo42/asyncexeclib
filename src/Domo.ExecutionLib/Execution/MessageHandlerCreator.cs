namespace Domo.ExecutionLib.Execution
{
   using System;
   using System.Collections;
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
      private readonly IMessageHandlerScanner _scanner;

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
      /// Initializes a new instance of the <see cref="MessageHandlerCreator"/> class.
      /// </summary>
      public MessageHandlerCreator(IMessageHandlerScanner scanner, IBuilder builder)
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
      public IEnumerable<IMessageHandler<TMessage>> Create<TMessage>(TMessage message) where TMessage : IMessage
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

         // check for msg interface handlers
         var interfaces = msgType.GetInterfaces().Where(x => typeof(IMessage).IsAssignableFrom(x));
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
            _builder.ConfigureMsgHandlerType(handlerType);
         }

         return handlerTypes;
      }

      /// <summary>
      /// Searches for all handlers and creates a handler chain called if a specfici message is
      /// to be handled.
      /// </summary>
      private void GroupHandlersByHandledTypes()
      {
         IEnumerable<Type> messageHandlerTypes = _scanner.Scan();
         foreach (Type msgHandler in messageHandlerTypes)
         {
            var handlerInterfaceTypes = msgHandler.GetInterfaces().Where(IsMessageHandlerInterface);
            foreach (Type handlerInterface in handlerInterfaceTypes)
            {
               Type handledMsgType = handlerInterface.GetGenericArguments().First();

               List<Type> handlerTypes;
               if (!_scannedHandlers.TryGetValue(handledMsgType, out handlerTypes))
               {
                  handlerTypes = new List<Type>();
                  _scannedHandlers.Add(handledMsgType, handlerTypes);
               }

               handlerTypes.Add(msgHandler);
            }
         }
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
