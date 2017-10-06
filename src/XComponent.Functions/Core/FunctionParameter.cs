using System;

namespace XComponent.Functions.Core
{
    public class FunctionParameter
    {
        public FunctionParameter()
        {
            RequestId = Guid.NewGuid().ToString();
        }
        public object Event { get; set; }
        public object PublicMember { get; set; }
        public object InternalMember { get; set; }
        public object Context { get; set; }
        public string ComponentName { get; set; }
        public string StateMachineName { get; set; }
        public string StateName { get; set; }
        public string RequestId { get; set; } 
    }
}
