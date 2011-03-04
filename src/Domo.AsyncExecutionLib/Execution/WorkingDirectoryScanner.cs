namespace Domo.AsyncExecutionLib.Execution
{
   using System;
   using System.Collections.Generic;
   using System.IO;
   using System.Linq;
   using System.Reflection;
   using log4net;

   /// <summary>
   /// Scans all assemblies of the current working directory for
   /// Message handlers.
   /// </summary>
   public class WorkingDirectoryScanner : IMessageHandlerScanner
   {
      /// <summary>
      /// Used for logging.
      /// </summary>
      private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      /// <summary>
      /// Scan for message handlers.
      /// </summary>
      /// <returns>List of available handlers.</returns>
      public IEnumerable<Type> Scan()
      {
         List<Type> handlerTypes = new List<Type>();

         string currentDir = Environment.CurrentDirectory;
         foreach (string file in Directory.GetFiles(currentDir, "*.dll"))
         {
             handlerTypes.AddRange(ScanPossibleAssembly(file));
         }

         foreach (string file in Directory.GetFiles(currentDir, "*.exe"))
         {
            handlerTypes.AddRange(ScanPossibleAssembly(file));
         }

         return handlerTypes;
      }

      /// <summary>
      /// Scan the file, which may or may not be an actual assembly.
      /// </summary>
      /// <param name="fileName"></param>
      private IEnumerable<Type> ScanPossibleAssembly(string fileName)
      {
         IEnumerable<Type> handlers = null;

         try
         {
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(fileName);
            Assembly assembly = Assembly.Load(assemblyName);
            handlers = ScanAssembly(assembly);
         }
         catch (BadImageFormatException)
         {
            _log.DebugFormat("File is not a .net assembly: {0}", fileName);
         }

         return handlers ?? new Type[0];
      }

      /// <summary>
      /// Scans an assembly for message handler implementations.
      /// </summary>
      /// <param name="assembly">The assembly to scan.</param>
      private IEnumerable<Type> ScanAssembly(Assembly assembly)
      {
         List<Type> handlerTypes = new List<Type>();

         _log.InfoFormat("Scanning assembly for message handlers: {0}", assembly.FullName);

         foreach (Type t in assembly.GetTypes())
         {
            if (!t.IsAbstract)
            {
               Type[] interfaces = t.GetInterfaces();
               if (interfaces.Any(IsMessageHandlerInterface))
               {
                  handlerTypes.Add(t);
               }
            }
         }

         return handlerTypes;
      }

      /// <summary>
      /// Returns true if type implements at least one message handler.
      /// </summary>
      /// <returns>True if implemented.</returns>
      private bool IsMessageHandlerInterface(Type interfaceType)
      {
         return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IMessageHandler<>);
      }
   }
}
