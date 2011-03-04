namespace Domo.AsyncExecutionLib.Execution
{
   using System;
   using System.Collections.Concurrent;
   using System.Threading;
   using log4net;

   /// <summary>
   /// Executes messages handlers on a single worker thread.
   /// </summary>
   public class SingleThreadQueue : IExecutionQueue
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
      /// Creates a new instance of the <see cref="SingleThreadQueue"/> class.
      /// </summary>
      public SingleThreadQueue()
      {
         var execThread = new Thread(ExecutionThread);
         execThread.IsBackground = true;
         execThread.Start();
      }

      /// <summary>
      /// Adds a message into the queue to be handled.
      /// </summary>
      /// <param name="job">Input message.</param>
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
      /// Thread delegate executing the message handlers.
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
