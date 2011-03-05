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

namespace Domo.AsyncExecutionLib.Execution
{
   using System;
   using System.Collections.Concurrent;

   /// <summary>
   /// Relays incoming message to the execution queue.
   /// </summary>
   public class ExecutionModule : IExecutionModule
   {
      /// <summary>
      /// Executes jobs on a separate thread.
      /// </summary>
      private readonly IExecutionQueue _execQueue;

      /// <summary>
      /// Creates message handlers based on message type.
      /// </summary>
      private readonly IMessageHandlerCreator _handlerCreator;

      /// <summary>
      /// Closed handler job types based on message types.
      /// </summary>
      private readonly ConcurrentDictionary<Type, Type> _handlerJobTypes = new ConcurrentDictionary<Type,Type>();

      /// <summary>
      /// Initializes a new instance of the <see cref="ExecutionModule"/> class.
      /// </summary>
      public ExecutionModule(IExecutionQueue execQueue, IMessageHandlerCreator handlerCreator)
      {
         _execQueue = execQueue;
         _handlerCreator = handlerCreator;
      }

      /// <summary>
      /// Add a message to be handled.
      /// </summary>
      /// <param name="message">The message</param>
      public void Add(IMessage message)
      {
         Type msgType = message.GetType();
         Type jobType = _handlerJobTypes.GetOrAdd(msgType, t => typeof(MessageHandlerExecutionJob<>).MakeGenericType(t));

         IJob job = (IJob)Activator.CreateInstance(jobType, message, _handlerCreator);
         _execQueue.Add(job);
      }

      /// <summary>
      /// Action to be executed.
      /// </summary>
      /// <param name="action">The action delegate to execute.</param>
      public void Add(Action action)
      {
         _execQueue.Add(new ActionExecutionJob(action));
      }
   }
}
