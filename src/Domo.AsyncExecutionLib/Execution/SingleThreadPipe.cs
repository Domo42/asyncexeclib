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

namespace Domo.AsyncExecutionLib.Execution
{
   using System;
   using System.Collections.Concurrent;
   using System.Threading;
   using log4net;

   /// <summary>
   /// Executes incoming jobs on a single worker thread.
   /// </summary>
   public class SingleThreadPipe : IExecutionPipe
   {
      /// <summary>
      /// Used for logging.
      /// </summary>
      private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      /// <summary>
      /// List of added but not yet handled messages;
      /// </summary>
      private readonly BlockingCollection<IJob> _jobs = new BlockingCollection<IJob>();

      /// <summary>
      /// Indicates whether this instance is already disposed.
      /// </summary>
      private bool _isDisposed;

      /// <summary>
      /// Creates a new instance of the <see cref="SingleThreadPipe"/> class.
      /// </summary>
      public SingleThreadPipe()
      {
         var execThread = new Thread(ExecutionThread);
         execThread.IsBackground = true;
         execThread.Start();
      }

      /// <summary>
      /// Adds a message into the pipe to be handled.
      /// </summary>
      /// <param name="job">Input job.</param>
      public void Add(IJob job)
      {
         _jobs.Add(job);
      }

      /// <summary>
      /// Dispose current instance.
      /// </summary>
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      /// <summary>
      /// Thread delegate executing the jobs.
      /// </summary>
      private void ExecutionThread()
      {
         while (!_isDisposed)
         {
            try
            {
               IJob job = _jobs.Take();
               job.Execute();
            }
            catch (ObjectDisposedException)
            {
               // object has been disposed
               // we don't have anything to do, therefore leave lopp.
               break;
            }
            catch (Exception ex)
            {
               _log.Error("Execution of job failed.", ex);
            }
         }
      }

      /// <summary>
      /// Dispses instances resources.
      /// </summary>
      protected virtual void Dispose(bool disposing)
      {
         if (disposing && !_isDisposed)
         {
            _jobs.Dispose();
         }

         _isDisposed = true;
      }
   }
}
