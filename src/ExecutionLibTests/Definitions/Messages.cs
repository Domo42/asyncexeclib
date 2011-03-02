namespace ExecutionLibTests
{
   using Domo.ExecutionLib;

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
