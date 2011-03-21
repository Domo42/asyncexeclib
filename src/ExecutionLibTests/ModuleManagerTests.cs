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
   using System.Linq;
   using NUnit.Framework;
   using OnyxOx.AsyncExecutionLib;
   using OnyxOx.AsyncExecutionLib.Execution;
   using Rhino.Mocks;

   [TestFixture]
   public class ModuleManagerTests
   {
      /// <summary>
      /// System under test.
      /// </summary>
      private IModuleManager _sut;

      /// <summary>
      /// Mocked builder
      /// </summary>
      private IBuilder _builder;

      /// <summary>
      /// Mocked assembly scanner
      /// </summary>
      private IAssemblyScanner _scanner;

      /// <summary>
      /// Init for every test.
      /// </summary>
      [SetUp]
      public void Init()
      {
         _builder = MockRepository.GenerateMock<IBuilder>();
         _scanner = MockRepository.GenerateMock<IAssemblyScanner>();

         _builder.Stub(x => x.GetInstance(typeof(MessageModule))).Return(MockRepository.GenerateMock<IMessageModule>());
         _builder.Stub(x => x.GetInstance(typeof(AnotherMessageModule))).Return(MockRepository.GenerateMock<IMessageModule>());

         _sut = new ModuleManager(_scanner, _builder);
      }

      /// <summary>
      /// given => Assembly scanner interface returns null.
      /// when  => Module manager is constructed.
      /// then  => Do not throw exception.
      /// </summary>
      [Test]
      public void OnStart_ScannerReturnsNull_DoNotThrow()
      {
         // given
         _scanner.Stub(x => x.ScanForMessageModules()).Return(null);

         // when
         TestDelegate onStartAction = () => _sut.OnStart();

         // then
         Assert.DoesNotThrow(onStartAction);
      }

      /// <summary>
      /// given => Assembly scanner returns types.
      /// when  => OnStart is called.
      /// then  => Execute on start on module instances.
      /// </summary>
      [Test]
      public void OnStart_ScannerHasModules_ExecuteModulesFromScanner()
      {
         // given
         var modules = new[] { typeof(MessageModule), typeof(AnotherMessageModule) };
         _scanner.Stub(x => x.ScanForMessageModules()).Return(modules);

         // when
         _sut.OnStart();

         // then
         var module1 = (IMessageModule)_builder.GetInstance(modules[0]);
         module1.AssertWasCalled(x => x.OnStart());

         var module2 = (IMessageModule)_builder.GetInstance(modules[1]);
         module2.AssertWasCalled(x => x.OnStart());
      }

      /// <summary>
      /// given => Assembly scanner returns types, prefered order is different.
      /// when  => OnStart is called.
      /// then  => OnStart on modules called in prefered order.
      /// </summary>
      [Test]
      public void OnStart_ScannerHasModulesAndPreferredExecutionOrder_ExecuteInPreferredOrder()
      {
         // given
         bool module1Called = false, module2CalledBefore1 = false;
         var modules = new[] { typeof(MessageModule), typeof(AnotherMessageModule) };
         _scanner.Stub(x => x.ScanForMessageModules()).Return(modules);
         _sut.SetPreferredOrder(modules.Reverse().ToArray());
         var module1 = (IMessageModule)_builder.GetInstance(modules[0]);
         var module2 = (IMessageModule)_builder.GetInstance(modules[1]);

         Action module1Action = () => module1Called = true;
         module1.Stub(x => x.OnStart()).Do(module1Action);

         Action module2Action = () => module2CalledBefore1 = !module1Called;
         module2.Stub(x => x.OnStart()).Do(module2Action);

         // when
         _sut.OnStart();

         // then
         Assert.That(module2CalledBefore1, Is.True, "AnotherMessageModule.OnStart has not been called first.");
      }

      /// <summary>
      /// given => Message modules are configured.
      /// when  => OnFinished is called.
      /// then  => Call OnFinished on all module instances.
      /// </summary>
      [Test]
      public void OnFinished_ModulesConfigured_CallFinishedOnAllModules()
      {
         // given
         var modules = new[] { typeof(MessageModule), typeof(AnotherMessageModule) };
         _scanner.Stub(x => x.ScanForMessageModules()).Return(modules);

         // when
         _sut.OnStart();
         _sut.OnFinished();

         // then
         var module1 = (IMessageModule)_builder.GetInstance(modules[0]);
         module1.AssertWasCalled(x => x.OnFinished());

         var module2 = (IMessageModule)_builder.GetInstance(modules[1]);
         module2.AssertWasCalled(x => x.OnFinished());
      }

      /// <summary>
      /// given => Message modules are configured.
      /// when  => OnFinished is called.
      /// then  => Call OnFinished methods in reverted order to OnStart
      /// </summary>
      [Test]
      public void OnFinished_ModulesConfigured_CallFinishedInReverseOrder()
      {
         // given
         bool module1Called = false, module2CalledBefore1 = false;
         var modules = new[] { typeof(MessageModule), typeof(AnotherMessageModule) };
         _scanner.Stub(x => x.ScanForMessageModules()).Return(modules);
         var module1 = (IMessageModule)_builder.GetInstance(modules[0]);
         var module2 = (IMessageModule)_builder.GetInstance(modules[1]);

         Action module1Action = () => module1Called = true;
         module1.Stub(x => x.OnFinished()).Do(module1Action);

         Action module2Action = () => module2CalledBefore1 = !module1Called;
         module2.Stub(x => x.OnFinished()).Do(module2Action);

         // when
         _sut.OnStart();
         _sut.OnFinished();

         // then
         Assert.That(module2CalledBefore1, Is.True, "AnotherMessageModule.OnFinished has not been called first.");
      }

      /// <summary>
      /// given => Message modules are configured.
      /// when  => OnError is called.
      /// then  => Call OnError on all module instances.
      /// </summary>
      [Test]
      public void OnError_ModulesConfigured_CallOnErrorOnAllModules()
      {
         // given
         var exception = new Exception();
         var modules = new[] { typeof(MessageModule), typeof(AnotherMessageModule) };
         _scanner.Stub(x => x.ScanForMessageModules()).Return(modules);

         // when
         _sut.OnStart();
         _sut.OnError(exception);

         // then
         var module1 = (IMessageModule)_builder.GetInstance(modules[0]);
         module1.AssertWasCalled(x => x.OnError(exception));

         var module2 = (IMessageModule)_builder.GetInstance(modules[1]);
         module2.AssertWasCalled(x => x.OnError(exception));
      }

      /// <summary>
      /// given => Assembly scanner returns types.
      /// when  => OnStart is called more than once.
      /// then  => Assembly scanner only called once.
      /// </summary>
      [Test]
      public void OnStart_ScannerHasModules_ScanningPerformedOnlyOnce()
      {
         // given
         var modules = new[] { typeof(MessageModule), typeof(AnotherMessageModule) };
         _scanner.Stub(x => x.ScanForMessageModules()).Return(modules);

         // when
         _sut.OnStart();
         _sut.OnStart();

         // then
         _scanner.AssertWasCalled(x => x.ScanForMessageModules(), o => o.Repeat.Once());
      }
   }
}
