namespace ExecutionLibTests
{
   using System;
   using System.Collections.Generic;
   using System.IO;
   using System.Linq;
   using Domo.ExecutionLib;
   using Domo.ExecutionLib.Execution;
   using NUnit.Framework;

   [TestFixture]
   public class WorkingDirectoryScannerTests
   {
      /// <summary>
      /// System under test.
      /// </summary>
      private IMessageHandlerScanner _sut;

      /// <summary>
      /// Init for every test.
      /// </summary>
      [SetUp]
      public void Init()
      {
         _sut = new WorkingDirectoryScanner();
      }

      /// <summary>
      /// given => Under any condition
      /// when  => Scan is called.
      /// then  => do not return null.
      /// </summary>
      [Test]
      public void Scan_Any_DoesNotReturnNull()
      {
         // when
         var retVal = _sut.Scan();

         // then
         Assert.That(retVal, Is.Not.Null);
      }

      /// <summary>
      /// given => Working directory is location of current assembly
      /// when  => Scan is called.
      /// then  => Returns all handler types located in this assembly.
      /// </summary>
      [Test]
      public void Scan_WorkingDirectorySet_ReturnsHandlersOfThisAssembly()
      {
         // given
         var expectedTypes = GetCurrentAssemblyHandlers();
         Environment.CurrentDirectory = Path.GetDirectoryName(typeof(WorkingDirectoryScannerTests).Assembly.Location);

         // when
         var retVal = _sut.Scan();

         // then
         Assert.That(retVal, Is.EquivalentTo(expectedTypes));
      }
      
      private IEnumerable<Type> GetCurrentAssemblyHandlers()
      {
         return typeof(WorkingDirectoryScannerTests).Assembly.GetTypes()
            .Where(x => x.IsClass && x.GetInterfaces()
               .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IMessageHandler<>)).Count() > 0);
      }
   }
}
