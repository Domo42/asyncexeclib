namespace ExecutionLibTests
{
   using System;
   using OnyxOx.AsyncExecutionLib;

   public class MessageModule : IMessageModule
   {
      public void OnStart()
      {
      }

      public void OnFinished()
      {
      }

      public void OnError(Exception ex)
      {
      }
   }

   public class AnotherMessageModule : IMessageModule
   {
      public void OnStart()
      {
      }

      public void OnFinished()
      {
      }

      public void OnError(Exception ex)
      {
      }
   }
}
