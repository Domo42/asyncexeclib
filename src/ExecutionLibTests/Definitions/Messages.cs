namespace ExecutionLibTests
{
   using Domo.AsyncExecutionLib;

   /// <summary>
   /// Message base class.
   /// </summary>
   public abstract class MessageBase : IMessage
   {
   }

   public class MessageSuperClass : MessageBase
   {
   }

   public class SeparateMessage : IMessage
   {
   }
}
