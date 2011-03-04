namespace Domo.AsyncExecutionLib.Execution
{
   using System;

   /// <summary>
   /// Adds jobs to be executed.
   /// </summary>
   public interface IExecutionQueue : IDisposable
   {
      /// <summary>
      /// Adds a job into the queue to be executed.
      /// </summary>
      /// <param name="job">Job to exeucte.</param>
      void Add(IJob job);
   }
}
