namespace Domo.AsyncExecutionLib
{
   using System;
   using System.Collections.Generic;

   /// <summary>
   /// Scans for available message handler.
   /// </summary>
   public interface IMessageHandlerScanner
   {
      /// <summary>
      /// Scan for message handlers.
      /// </summary>
      /// <returns>List of available handlers.</returns>
      IEnumerable<Type> Scan();
   }
}
