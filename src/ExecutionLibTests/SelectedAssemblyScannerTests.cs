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

namespace ExecutionLibTests
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using NUnit.Framework;
   using OnyxOx.AsyncExecutionLib;
   using OnyxOx.AsyncExecutionLib.Execution;

   [TestFixture]
   public class SelectedAssemblyScannerTests
   {
      /// <summary>
      /// System under test.
      /// </summary>
      private SelectedAssemblyScanner _sut;

      /// <summary>
      /// Init for every test.
      /// </summary>
      [SetUp]
      public void Init()
      {
         _sut = new SelectedAssemblyScanner();
      }

      /// <summary>
      /// given => Current assembly added to scanner.
      /// when  => ScanForMessageHanlers callded.
      /// then  => Returns all handlers of this assembly.
      /// </summary>
      [Test]
      public void ScanForMessageHandlers_CurrentAssemblySet_ReturnLocalHandlertypes()
      {
         // given
         var expected = GetCurrentAssemblyHandlers().ToArray();
         _sut.SetAssembliesToScan(new[] { typeof(SelectedAssemblyScannerTests).Assembly });

         // when
         var retVal = _sut.ScanForMessageHandlers().ToArray();

         // then
         Assert.That(retVal, Is.EquivalentTo(expected));
      }

      /// <summary>
      /// given => No assemblies set to scan.
      /// when  => ScanForMessageHandlers called.
      /// then  => Returns an empty handler list.
      /// </summary>
      [Test]
      public void ScanForMessageHandlers_NoAssemblySet_ReturnEmptyList()
      {
         // given
         _sut.SetAssembliesToScan(null);

         // when
         var retVal = _sut.ScanForMessageHandlers().ToArray();

         // then
         Assert.That(retVal, Is.Empty);
      }

      /// <summary>
      /// given => Set current assembly to scan.
      /// when  => ScanForMessageModules called.
      /// then  => Returns current assembliesm modules.
      /// </summary>
      [Test]
      public void ScanForMessageModules_CurrentAssemblySet_ReturnLocalModules()
      {
         // given
         var expected = GetCurrentAssemblyModules().ToArray();
         _sut.SetAssembliesToScan(new[] { typeof(SelectedAssemblyScannerTests).Assembly });

         // when
         var retVal = _sut.ScanForMessageModules().ToArray();

         // then
         Assert.That(retVal, Is.EquivalentTo(expected));
      }

      /// <summary>
      /// given => Set no assembly to scan.
      /// when  => ScanForMessageModules called.
      /// then  => Returns empty list.
      /// </summary>
      [Test]
      public void ScanForMessageModules_NoAssemblySet_ReturnEmptyList()
      {
         // given
         _sut.SetAssembliesToScan(null);

         // when
         var retVal = _sut.ScanForMessageModules().ToArray();

         // then
         Assert.That(retVal, Is.Empty);
      }

      private IEnumerable<Type> GetCurrentAssemblyHandlers()
      {
         return typeof(WorkingDirectoryScannerTests).Assembly.GetTypes()
            .Where(x => x.IsClass && x.GetInterfaces()
               .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IMessageHandler<>)).Count() > 0);
      }

      private IEnumerable<Type> GetCurrentAssemblyModules()
      {
         return typeof(WorkingDirectoryScannerTests).Assembly.GetTypes()
            .Where(x => x.IsClass && x.GetInterfaces()
               .Where(type => typeof(IMessageModule).IsAssignableFrom(type)).Count() > 0);
      }
   }
}
