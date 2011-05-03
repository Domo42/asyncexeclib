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
   using OnyxOx.AsyncExecutionLib;
   using OnyxOx.AsyncExecutionLib.Execution;
   using NUnit.Framework;
   using Rhino.Mocks;

   [TestFixture]
   public class ExecutionModuleTests
   {
      /// <summary>
      /// System under test.
      /// </summary>
      private IExecutionModule _sut;

      /// <summary>
      /// Mocked execution pipe.
      /// </summary>
      private IExecutionPipe _execPipe;

      /// <summary>
      /// Mocked handler creator.
      /// </summary>
      private IMessageHandlerCreator _handlerCreator;

      /// <summary>
      /// The actual message handled.
      /// </summary>
      private IMessage _message;

      /// <summary>
      /// Message handler used to extract the msg instance given my module.
      /// </summary>
      private ExtractHandledMessageHandler _handler;

      /// <summary>
      /// Thread local execution context.
      /// </summary>
      private ILocalContext _context;

      /// <summary>
      /// Init every test.
      /// </summary>
      [SetUp]
      public void Init()
      {
         _handler = new ExtractHandledMessageHandler(m => _message = m);
         _execPipe = MockRepository.GenerateMock<IExecutionPipe>();
         _context = MockRepository.GenerateMock<ILocalContext>();

         _handlerCreator = MockRepository.GenerateMock<IMessageHandlerCreator>();
         _handlerCreator.Stub(x => x.Create<SeparateMessage>(null))
            .IgnoreArguments().Return(new [] { _handler });

         _sut = new ExecutionModule(_execPipe, _handlerCreator, MockRepository.GenerateStub<IModuleManager>(), MockRepository.GenerateStub<IAsyncLibLog>(), _context);
      }

      /// <summary>
      /// given => An imessage instance.
      /// when  => Add method called.
      /// then  => Message handler job added to pipe.
      /// </summary>
      [Test]
      public void Add_Message_AddMessageHandlerJobToPipe()
      {
         // given
         var msg = new SeparateMessage();

         // when
         _sut.Add(msg);

         // then
         var constraint = new ParamConstraint<MessageHandlerExecutionJob<SeparateMessage>>();
         _execPipe.AssertWasCalled(x => x.Add(Arg<IJob>.Matches(constraint)));
      }

      /// <summary>
      /// given => An SepratateMessage message.
      /// when  => Add method handler called.
      ///          And message handler executed.
      /// then  => Message handled is the same instance as added to module.
      /// </summary>
      [Test]
      public void Add_Message_SameMessageInstanceUsedInHandler()
      {
         // given
         var msg = new SeparateMessage();
         IJob job = null;
         Action<IJob> pipeAddAction = x => job = x;
         _execPipe.Stub(x => x.Add(null)).IgnoreArguments().Do(pipeAddAction);
         
         // when
         _sut.Add(msg);
         job.Execute();

         // then
         Assert.That(_message, Is.SameAs(msg));
      }

      /// <summary>
      /// given => An action delegate.
      /// when  => Add method called.
      /// then  => ActionExecution job is added to execution pipe.
      /// </summary>
      [Test]
      public void Add_Action_AddActionExecutionJobToPipe()
      {
         // given
         Action action = null;

         // when
         _sut.Add(action);

         // then
         var constraint = new ParamConstraint<ActionExecutionJob>();
         _execPipe.AssertWasCalled(x => x.Add(Arg<IJob>.Matches(constraint)));
      }

      /// <summary>
      /// given => An action to be executed.
      /// when  => Add method with action called.
      ///          Job is executed.
      /// then  => job executes given action.
      /// </summary>
      [Test]
      public void Add_Action_JobExecutesAction()
      {
         // given
         bool hasExecuted = false;
         Action action = () => hasExecuted = true;
         IJob job = null;
         Action<IJob> pipeAddAction = x => job = x;
         _execPipe.Stub(x => x.Add(null)).IgnoreArguments().Do(pipeAddAction);

         // when
         _sut.Add(action);
         job.Execute();

         // then
         Assert.That(hasExecuted, Is.True, "Job added to execution pipe has not executed the correct action.");
      }

      #region [ Support ]

      private class ExtractHandledMessageHandler : IMessageHandler<IMessage>
      {
         private readonly Action<IMessage> _extrator;

         public ExtractHandledMessageHandler(Action<IMessage> extractor)
         {
            _extrator = extractor;
         }

         public void Handle(IMessage message)
         {
            _extrator(message);
         }
      }

      #endregion
   }
}
