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

namespace OnyxOx.AsyncExecutionLib.Execution
{
   using System;
   using System.Collections.Concurrent;

   /// <summary>
   /// Relays incoming message to the execution pipe.
   /// </summary>
   public class ExecutionModule : IExecutionModule
   {
      /// <summary>
      /// Used for logging.
      /// </summary>
      private readonly IAsyncLibLog _log;

      /// <summary>
      /// Executes jobs on a separate thread.
      /// </summary>
      private readonly IExecutionPipe _execPipe;

      /// <summary>
      /// Creates message handlers based on message type.
      /// </summary>
      private readonly IMessageHandlerCreator _handlerCreator;

      /// <summary>
      /// Managers modules.
      /// </summary>
      private readonly IModuleManager _moduleManager;

      /// <summary>
      /// Message execution context.
      /// </summary>
      private readonly ILocalContext _context;

      /// <summary>
      /// Closed handler job types based on message types.
      /// </summary>
      private readonly ConcurrentDictionary<Type, Type> _handlerJobTypes = new ConcurrentDictionary<Type,Type>();

      /// <summary>
      /// Initializes a new instance of the <see cref="ExecutionModule"/> class.
      /// </summary>
      public ExecutionModule(
            IExecutionPipe execPipe,
            IMessageHandlerCreator handlerCreator,
            IModuleManager moduleManager,
            IAsyncLibLog log,
            ILocalContext context)
      {
         _execPipe = execPipe;
         _handlerCreator = handlerCreator;
         _moduleManager = moduleManager;
         _log = log;
         _context = context;
      }

      /// <summary>
      /// Add a message to be handled.
      /// </summary>
      /// <param name="message">The message</param>
      public virtual void Add(IMessage message)
      {
         IJob job = CreateMessageHandlerExecutionJob(message);
         _execPipe.Add(job);
      }

      /// <summary>
      /// Action to be executed.
      /// </summary>
      /// <param name="action">The action delegate to execute.</param>
      public virtual void Add(Action action)
      {
         _execPipe.Add(new ActionExecutionJob(action, _moduleManager, _log, _context));
      }

      /// <summary>
      /// Handle the message in a synchron operation. This will not put
      /// the message into the configured execution pipe.
      /// </summary>
      /// <param name="message">The message to handle.</param>
      public void Handle(IMessage message)
      {
         IJob job = CreateMessageHandlerExecutionJob(message);
         job.Execute();
      }

      /// <summary>
      /// Creates the job for message handler execution.
      /// </summary>
      /// <returns>The created job instance.</returns>
      private IJob CreateMessageHandlerExecutionJob(IMessage message)
      {
         Type msgType = message.GetType();
         Type jobType = _handlerJobTypes.GetOrAdd(msgType, t => typeof(MessageHandlerExecutionJob<>).MakeGenericType(t));

         IJob job = (IJob)Activator.CreateInstance(jobType, message, _handlerCreator, _moduleManager, _log, _context);
         return job;
      }
   }
}
