using System;
using System.Reflection;
using Newtonsoft.Json;

namespace XComponent.Functions.Utilities
{
    internal static class SerializationHelper
    {
        public static string SerializeObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            try
            {
                string serializeObject = JsonConvert.SerializeObject(obj);
                return serializeObject;
            }
            catch (Exception e)
            {
            }

            return null;
        }

        public static T DeserializeObject<T>(string obj) where T:class
        {
            if (string.IsNullOrEmpty(obj))
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(obj);
            }
            catch (Exception e)
            {
               
            }
            return null;
        }

        public static object DeserializeObjectFromType(Type objType, object objectToDeserialize)
        {
            if (objectToDeserialize == null)
            {
                return null;
            }
            try
            {
                return typeof(SerializationHelper).GetMethod(nameof(DeserializeObject),
                        BindingFlags.Public | BindingFlags.Static)
                    .MakeGenericMethod(new[] { objType })
                    .Invoke(null, new[] { objectToDeserialize });
            }
            catch (Exception e)
            {
               
            }

            return null;

        }
    }
}
