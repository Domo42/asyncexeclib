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
   /// Executes a single action delegate.
   /// </summary>
   public class ActionExecutionJob : IJob
   {
      /// <summary>
      /// Action to be executed.
      /// </summary>
      private readonly Action _action;

      /// <summary>
      /// Responsible to call modules.
      /// </summary>
      private readonly IModuleManager _moduleManager;

      /// <summary>
      /// Used for logging.
      /// </summary>
      private readonly IAsyncLibLog _log;

      /// <summary>
      /// Initializes a new instance of the <see cref="ActionExecutionJob"/> class.
      /// </summary>
      public ActionExecutionJob(Action action, IModuleManager moduleManager, IAsyncLibLog log)
      {
         _action = action;
         _moduleManager = moduleManager;
         _log = log;
      }

      /// <summary>
      /// Execute the job.
      /// </summary>
      public void Execute()
      {
         _moduleManager.OnStart();

         try
         {
            _action.Invoke();
         }
         catch (Exception ex)
         {
            _log.Error("Error execution action.", ex);
            _moduleManager.OnError(ex);
         }
         finally
         {
            _moduleManager.OnFinished();
         }
      }
   }
}
