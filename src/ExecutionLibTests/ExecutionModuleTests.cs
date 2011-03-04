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
   using Domo.AsyncExecutionLib;
   using Domo.AsyncExecutionLib.Execution;
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
      /// Mocked execution queue.
      /// </summary>
      private IExecutionQueue _execQueue;

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
      /// Init every test.
      /// </summary>
      [SetUp]
      public void Init()
      {
         _handler = new ExtractHandledMessageHandler(m => _message = m);
         _execQueue = MockRepository.GenerateMock<IExecutionQueue>();

         _handlerCreator = MockRepository.GenerateMock<IMessageHandlerCreator>();
         _handlerCreator.Stub(x => x.Create<SeparateMessage>(null))
            .IgnoreArguments().Return(new [] { _handler });

         _sut = new ExecutionModule(_execQueue, _handlerCreator);
      }

      /// <summary>
      /// given => An imessage instance.
      /// when  => Add method called.
      /// then  => Message handler job added to queue.
      /// </summary>
      [Test]
      public void AddMessage_Message_AddMessageHandlerJobToQueue()
      {
         // given
         var msg = new SeparateMessage();

         // when
         _sut.Add(msg);

         // then
         var constraint = new ParamConstraint<MessageHandlerExecutionJob<SeparateMessage>>();
         _execQueue.AssertWasCalled(x => x.Add(Arg<IJob>.Matches(constraint)));
      }

      /// <summary>
      /// given => An SepratateMessage message.
      /// when  => Add method handler called.
      ///          And message handler executed.
      /// then  => Message handled is the same instance as added to module.
      /// </summary>
      [Test]
      public void AddMessage_Message_SameMessageInstanceUsedInHandler()
      {
         // given
         var msg = new SeparateMessage();
         IJob job = null;
         Action<IJob> queueAddAction = x => job = x;
         _execQueue.Stub(x => x.Add(null)).IgnoreArguments().Do(queueAddAction);
         
         // when
         _sut.Add(msg);
         job.Execute();

         // then
         Assert.That(_message, Is.SameAs(msg));
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
