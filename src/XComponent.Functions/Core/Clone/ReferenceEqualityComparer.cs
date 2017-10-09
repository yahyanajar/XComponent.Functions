using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace XComponent.Functions.Core.Clone
{
    internal class ReferenceEqualityComparer : EqualityComparer<object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
