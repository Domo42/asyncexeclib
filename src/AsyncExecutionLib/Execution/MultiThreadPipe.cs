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
   using System.Threading;

   /// <summary>
   /// Execution pipe using multiple worker threads.
   /// </summary>
   public class MultiThreadPipe : IExecutionPipe
   {
      /// <summary>
      /// Used for logging.
      /// </summary>
      private readonly IAsyncLibLog _log;

      /// <summary>
      /// List of jobs to execute.
      /// </summary>
      private readonly BlockingCollection<IJob> _jobs = new BlockingCollection<IJob>();

      /// <summary>
      /// Indicates whether this instance is already disposed.
      /// </summary>
      private bool _isDisposed;

      /// <summary>
      /// Initializes a new instance of the <see cref="MultiThreadPipe"/> class.
      /// </summary>
      public MultiThreadPipe(IAsyncLibLog log)
      {
         _log = log;

         // have at least one worker.
         IncreaseWorkerThreads(1);
      }

      /// <summary>
      /// Adds a job into the pipe to be executed.
      /// </summary>
      /// <param name="job">Job to exeucte.</param>
      public void Add(IJob job)
      {
         _jobs.Add(job);
      }

      /// <summary>
      /// Increases the number of worker threads used to execute jobs.
      /// </summary>
      /// <param name="newWorkers">Number of worker threads to create.</param>
      public void IncreaseWorkerThreads(int newWorkers)
      {
         for (int i = 0; i < newWorkers; ++i)
         {
            var execThread = new Thread(ExecutionThread);
            execThread.IsBackground = true;
            execThread.Start();
         }
      }

      /// <summary>
      /// Thread delegate executing the jobs.
      /// </summary>
      private void ExecutionThread()
      {
         try
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
                  _log.Error(@"Execution of job failed.", ex);
               }
            }
         }
         finally
         {
            // this finally block will always get executed.
            _log.Info(@"Worker thread closed.");
         }
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      /// <summary>
      /// Disposes instances resources.
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
