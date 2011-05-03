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
   using System.Threading;
   using NUnit.Framework;
   using OnyxOx.AsyncExecutionLib;
   using OnyxOx.AsyncExecutionLib.Execution;
   using Rhino.Mocks;

   [TestFixture]
   public class MultiThreadPipeTests
   {
      /// <summary>
      /// System under test.
      /// </summary>
      private MultiThreadPipe _sut;

      /// <summary>
      /// Mocked logger.
      /// </summary>
      private IAsyncLibLog _log;

      /// <summary>
      /// Mocked module manager.
      /// </summary>
      private IModuleManager _moduleManager;

      /// <summary>
      /// Mocked execution context.
      /// </summary>
      private ILocalContext _context;
      

      /// <summary>
      /// Init for every test.
      /// </summary>
      [SetUp]
      public void Init()
      {
         _log = MockRepository.GenerateStub<IAsyncLibLog>();
         _moduleManager = MockRepository.GenerateStub<IModuleManager>();
         _context = MockRepository.GenerateStub<ILocalContext>();

         _sut = new MultiThreadPipe(_log);
      }

      /// <summary>
      /// given => new job created.
      /// when  => Job added to queue.
      /// then  => Job execute must be called.
      /// </summary>
      [Test]
      public void Add_NewJob_JobGetsExecuted()
      {
         // given
         bool hasBeenExecuted = false;
         var waitEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
         Action action = () => { hasBeenExecuted = true; waitEvent.Set(); };
         var job = new ActionExecutionJob(action, _moduleManager, _log, _context);

         // when
         _sut.Add(job);

         // then
         if (waitEvent.WaitOne(TimeSpan.FromSeconds(1)))
         {
            Assert.That(hasBeenExecuted, Is.True, "Job has not been executed.");
         }
         else
         {
            Assert.Fail("Job not executed in target timeframe.");
         }
      }
   }
}
