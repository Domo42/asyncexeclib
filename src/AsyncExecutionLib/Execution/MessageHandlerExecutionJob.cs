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

   /// <summary>
   /// Executes message handlers for the given message.
   /// </summary>
   /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
   public class MessageHandlerExecutionJob<TMessage> : IJob where TMessage : IMessage
   {
      /// <summary>
      /// The message to handle.
      /// </summary>
      private readonly TMessage _message;

      /// <summary>
      /// Creates message handlers for the message.
      /// </summary>
      private readonly IMessageHandlerCreator _handlerCreator;

      /// <summary>
      /// Respsonsible to for message modules.
      /// </summary>
      private readonly IModuleManager _moduleManager;

      /// <summary>
      /// The logger.
      /// </summary>
      private readonly IAsyncLibLog _log;

      /// <summary>
      /// The thread local execution context.
      /// </summary>
      private readonly ILocalContext _context;

      /// <summary>
      /// Creates a new <see cref="MessageHandlerExecutionJob&lt;T&gt;"/> instance.
      /// </summary>
      public MessageHandlerExecutionJob(
         TMessage message,
         IMessageHandlerCreator handleCreator,
         IModuleManager moduleManager,
         IAsyncLibLog log,
         ILocalContext context)
      {
         _message = message;
         _handlerCreator = handleCreator;
         _moduleManager = moduleManager;
         _log = log;
         _context = context;
      }

      /// <summary>
      /// Execute the job.
      /// </summary>
      public void Execute()
      {
         try
         {
            InitExecutionContext();

            _moduleManager.OnStart();
            var handlers = _handlerCreator.Create(_message);

            foreach (var handler in handlers)
            {
               handler.Handle(_message);
            }
         }
         catch (Exception ex)
         {
            _log.Error(string.Format("Error handling message {0}.", typeof(TMessage).Name), ex);
            _moduleManager.OnError(ex);
         }
         finally
         {
            _moduleManager.OnFinished();
         }
      }

      /// <summary>
      /// Initialize thread local execution context.
      /// </summary>
      private void InitExecutionContext()
      {
         LocalContext context = _context as LocalContext;
         if (context != null)
         {
            context.Initialize(_message);
         }
      }
   }
}
