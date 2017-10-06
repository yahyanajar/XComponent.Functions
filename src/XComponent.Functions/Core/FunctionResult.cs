using XComponent.Functions.Core.Senders;

namespace XComponent.Functions.Core
{
    public class FunctionResult
    {
        public string ComponentName { get; set; }
        public string StateMachineName { get; set; }
        public object PublicMember { get; set; }
        public object InternalMember { get; set; }
        public SenderResult Sender { get; set; }
        public string RequestId { get; set; }

    }
}
