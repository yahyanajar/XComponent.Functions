using System;
using System.Collections.Generic;
using System.Reflection;

namespace XComponent.Functions.Core.Senders
{
    internal class SenderMethod
    {
        public string Name { get; set; }
        public IEnumerable<SenderParameter> SenderParameterCollection { get; set; }
        public MethodInfo MethodInfo { get; set; }
    }

    internal class SenderParameter
    {
        public Type Type { get; set; }
        public string Name { get; set; }
    }
}
