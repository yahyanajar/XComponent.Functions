using System;

namespace XComponent.Functions.Core.Senders
{
    [Serializable]
    public class SenderResult
    {
        public string SenderName { get; set; }
        public object SenderParameter { get; set; }
        public bool UseContext { get; set; }
    }
}
