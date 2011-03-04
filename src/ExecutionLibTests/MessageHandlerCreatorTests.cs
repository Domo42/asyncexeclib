﻿#region [ File Header ]
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
   using System.Linq;
   using Domo.AsyncExecutionLib;
   using Domo.AsyncExecutionLib.Execution;
   using NUnit.Framework;
   using Rhino.Mocks;

   [TestFixture]
   public class MessageHandlerCreatorTests
   {
      /// <summary>
      /// System under test.
      /// </summary>
      private IMessageHandlerCreator _sut;

      /// <summary>
      /// Instance builder;
      /// </summary>
      private IBuilder _builder;

      /// <summary>
      /// Setup for single test.
      /// </summary>
      [SetUp]
      public void Init()
      {
         var scanner = MockRepository.GenerateStub<IMessageHandlerScanner>();
         scanner.Stub(x => x.Scan()).Return(new[] { typeof(SeparateMessgeHandler), typeof(InterfaceMessageHandler), typeof(MessageBaseHandler) });

         _builder = new StructureMapBuilder();

         _sut = new MessageHandlerCreator(scanner, _builder);
      }

      /// <summary>
      /// given => Null as method param
      /// when  => Create is called.
      /// then  => returns non null collection.
      /// </summary>
      [Test]
      public void Create_NullMessage_ReturnsNotNull()
      {
         // given
         IMessage msg = null;

         // when
         var retVal = _sut.Create(msg);

         // then
         Assert.That(retVal, Is.Not.Null);
      }

      /// <summary>
      /// given => Use SeparateMessage msg
      /// when  => Create is called
      /// then  => Returns list containe separate handler and IMessage handler.
      /// </summary>
      [Test]
      public void Create_SeparateMessage_ReturnsSeparateMsgHandlerAndIMessageHandler()
      {
         // given
         var msg = new SeparateMessage();

         // when
         var retVal = _sut.Create(msg).ToArray();

         // then
         Assert.That(retVal, Has.Some.InstanceOf<InterfaceMessageHandler>());
         Assert.That(retVal, Has.Some.InstanceOf<SeparateMessgeHandler>());
      }

      /// <summary>
      /// given => MessageSuperClass msg
      /// when  => Create is called
      /// then  => Returns list container base msg handler and IMessage handler.
      /// </summary>
      [Test]
      public void Create_SuperClassMessage_ReturnsMsgBaseHandlerAndIMessageHandlers()
      {
         // given
         var msg = new MessageSuperClass();

         // when
         var retVal = _sut.Create(msg).ToArray();

         // then
         Assert.That(retVal, Has.Some.InstanceOf<InterfaceMessageHandler>());
         Assert.That(retVal, Has.Some.InstanceOf<MessageBaseHandler>());
      }
   }
}
