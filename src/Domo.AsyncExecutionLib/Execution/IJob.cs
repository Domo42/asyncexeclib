namespace Domo.AsyncExecutionLib.Execution
{
   /// <summary>
   /// A job to executed in the execution lib. Typical job execute
   /// a list of message handlers, or action delegate.
   /// </summary>
   public interface IJob
   {
      /// <summary>
      /// Execute the job.
      /// </summary>
      void Execute();
   }
}
