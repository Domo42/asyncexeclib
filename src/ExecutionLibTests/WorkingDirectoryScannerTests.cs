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
   using Domo.AsyncExecutionLib;
   using Domo.AsyncExecutionLib.Execution;
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
