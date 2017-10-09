using System;

namespace XComponent.Functions.Core.Clone
{
    internal static class XCClone
    {
        public static T Clone<T>(T objectToClone) where T : class
        {
            ICloneable cloneable = objectToClone as ICloneable;
            if (cloneable != null)
            {
                return (T) cloneable.Clone();
            }

            return DeepCopyClone(objectToClone);
        }

        public static T DeepCopyClone<T>(T objectToClone) where T : class
        {
            return RecursiveMemberwiseCopy(objectToClone);
        }

        private static T RecursiveMemberwiseCopy<T>(T objectToClone)
        {
            return objectToClone.Copy();
        }

        public static T Clone<T>(this T original, T targetObject) where T : class
        {
            return (T) original.Copy((Object) targetObject);
        }
    }
}
