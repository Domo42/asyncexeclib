namespace ExecutionLibTests
{
   using Domo.ExecutionLib;
   using Domo.ExecutionLib.Execution;
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
      /// Setup for single test.
      /// </summary>
      [SetUp]
      public void Init()
      {
         var scanner = MockRepository.GenerateStub<IMessageHandlerScanner>();
         scanner.Stub(x => x.Scan()).Return(new[] { typeof(SeparateMessgeHandler), typeof(InterfaceMessageHandler), typeof(MessageBaseHandler) });

         _sut = new MessageHandlerCreator(scanner);
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
         var retVal = _sut.Create(msg);

         // then
         Assert.That(retVal, Has.Some.InstanceOf<InterfaceMessageHandler>());
         Assert.That(retVal, Has.Some.InstanceOf<SeparateMessgeHandler>());
      }
   }
}
