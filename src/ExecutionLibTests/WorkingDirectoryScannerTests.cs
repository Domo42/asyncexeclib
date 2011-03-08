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
   using System.IO;
   using System.Linq;
   using OnyxOx.AsyncExecutionLib;
   using OnyxOx.AsyncExecutionLib.Execution;
   using NUnit.Framework;

   [TestFixture]
   public class WorkingDirectoryScannerTests
   {
      /// <summary>
      /// System under test.
      /// </summary>
      private IAssemblyScanner _sut;

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
      /// when  => Scan message handler is called.
      /// then  => do not return null.
      /// </summary>
      [Test]
      public void Scan_Any_DoesNotReturnNull()
      {
         // when
         var retVal = _sut.ScanForMessageHandlers();

         // then
         Assert.That(retVal, Is.Not.Null);
      }

      /// <summary>
      /// given => Working directory is location of current assembly
      /// when  => Scan message handlers is called.
      /// then  => Returns all handler types located in this assembly.
      /// </summary>
      [Test]
      public void Scan_WorkingDirectorySet_ReturnsHandlersOfThisAssembly()
      {
         // given
         var expectedTypes = GetCurrentAssemblyHandlers().ToArray();
         Environment.CurrentDirectory = Path.GetDirectoryName(typeof(WorkingDirectoryScannerTests).Assembly.Location);

         // when
         var retVal = _sut.ScanForMessageHandlers().ToArray();

         // then
         Assert.That(retVal, Is.EquivalentTo(expectedTypes));
      }

      /// <summary>
      /// given => Working directory is locatio nof current assembly.
      /// when  => Scan message modules called.
      /// then  => Returns all handlers located in this assembly.
      /// </summary>
      [Test]
      public void Scan_WorkingDirectorySet_ReturnsModulesOfThisAssembly()
      {
         // given
         var expectedTypes = GetCurrentAssemblyModules().ToArray();
         Environment.CurrentDirectory = Path.GetDirectoryName(typeof(WorkingDirectoryScannerTests).Assembly.Location);

         // when
         var retval = _sut.ScanForMessageModules().ToArray();

         // then
         Assert.That(retval, Is.EquivalentTo(expectedTypes));
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
