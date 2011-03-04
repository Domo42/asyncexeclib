namespace Domo.AsyncExecutionLib
{
   using System;

   /// <summary>
   /// Builds new object instances.
   /// </summary>
   public interface IBuilder
   {
      /// <summary>
      /// Returns instance of the requested type.
      /// </summary>
      /// <param name="type">The type of the requested instance.</param>
      /// <returns>Type instance.</returns>
      object GetInstance(Type type);

      /// <summary>
      /// Inform builder of a message handler type to build.
      /// </summary>
      /// <param name="msgHandlerType">Target type</param>
      void ConfigureMsgHandlerType(Type msgHandlerType);
   }
}
