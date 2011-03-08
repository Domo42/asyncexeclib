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
   using NUnit.Framework;
   using OnyxOx.AsyncExecutionLib;
   using OnyxOx.AsyncExecutionLib.Execution;
   using Rhino.Mocks;

   [TestFixture]
   public class ModuleManagerTests
   {
      /// <summary>
      /// Mocked builder
      /// </summary>
      private IBuilder _builder;

      /// <summary>
      /// Init for every test.
      /// </summary>
      public void Init()
      {
         _builder = MockRepository.GenerateMock<IBuilder>();
      }

      /// <summary>
      /// given => Assembly scanner interface returns null.
      /// when  => Module manager is constructed.
      /// then  => Do not throw exception.
      /// </summary>
      [Test]
      public void Ctor_ScannerReturnsNull_DoNotThrow()
      {
         // given
         IAssemblyScanner scanner = MockRepository.GenerateMock<IAssemblyScanner>();
         scanner.Stub(x => x.ScanForMessageModules()).Return(null);

         // when
         TestDelegate ctorAction = () => new ModuleManager(scanner, _builder);

         // then
         Assert.DoesNotThrow(ctorAction);
      }
   }
}
