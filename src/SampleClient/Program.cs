namespace SampleClient
{
   using Domo.AsyncExecutionLib;

   /// <summary>
   /// Main program entry class.
   /// </summary>
   class Program
   {
      /// <summary>
      /// Main program entry point.
      /// </summary>
      static void Main()
      {
         Module.Configure()
            .UseStructureMap();
      }
   }
}
