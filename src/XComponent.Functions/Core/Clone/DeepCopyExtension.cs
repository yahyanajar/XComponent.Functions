using System;
using System.Collections.Generic;
using System.Reflection;

namespace XComponent.Functions.Core.Clone
{
    internal static class DeepCopyExtension
    {
        private static readonly MethodInfo CloneMethod =
            typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool ShouldCloneDeeply(this Type type)
        {
            if (type == typeof(String)) return true;
            if (type == typeof(Decimal)) return true;
            if (type == typeof(DateTime)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }

        public static Object Copy(this Object originalObject, Object targetObject)
        {
            return InternalCopyKeepReference(originalObject, targetObject,
                new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }

        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (ShouldCloneDeeply(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (ShouldCloneDeeply(arrayType) == false)
                {
                    Array clonedArray = (Array) cloneObject;
                    clonedArray.ForEach(
                        (array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited),
                            indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect, false);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect, false);
            return cloneObject;
        }

        private static Object InternalCopyKeepReference(Object originalObject, Object targetObject,
            IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (ShouldCloneDeeply(arrayType))
                {
                    Array originArray = (Array) originalObject;
                    Array clonedArray = (Array) targetObject;
                    clonedArray.ForEach(
                        (array, indices) => array.SetValue(InternalCopy(originArray.GetValue(indices), visited),
                            indices));
                }

            }
            visited.Add(originalObject, targetObject);
            CopyFields(originalObject, visited, targetObject, typeToReflect, true);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, targetObject, typeToReflect, true);
            return targetObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject,
            IDictionary<object, object> visited, object cloneObject, Type typeToReflect, bool copyAllFields)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType,
                    copyAllFields);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, copyAllFields,
                    BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject,
            Type typeToReflect, bool copyAllFields,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                        BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (ShouldCloneDeeply(fieldInfo.FieldType) && !copyAllFields) continue;
                if (fieldInfo.GetCustomAttributes(typeof(System.NonSerializedAttribute), false).Length > 0) continue;
                if (fieldInfo.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), false).Length >
                    0) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        public static T Copy<T>(this T original)
        {
            return (T) Copy((Object) original);
        }

        public static T Copy<T>(this T original, T targetObject)
        {
            return (T) Copy((Object) original, (Object) targetObject);
        }
    }
}
