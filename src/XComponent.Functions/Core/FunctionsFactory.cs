using System;
using System.Collections.Generic;

namespace XComponent.Functions.Core
{

    public static class FunctionsFactory
    {
        private static readonly Dictionary<int, FunctionsManager> _functionsFactoryByKey = new Dictionary<int, FunctionsManager>();

        public static FunctionsManager CreateFunctionsManager(string componentName, string stateMachineName, FunctionsProtocol protocol = FunctionsProtocol.http, string host = "127.0.0.1", int port = 9756)
        {

            var functionsManager = new FunctionsManager(componentName, stateMachineName);

            lock (_functionsFactoryByKey)
            {
                int key = GetFunctionsManagerKey(componentName, stateMachineName);
                if (_functionsFactoryByKey.ContainsKey(key))
                {
                    throw  new Exception("A function manager is already registered for: " + componentName + "," + stateMachineName);
                }
                _functionsFactoryByKey.Add(key, functionsManager);
            }
            functionsManager.InitManager(protocol, host, port);

            
            return functionsManager;
        }

        public static void UnRegisterFunctionsManager(FunctionsManager functionManager)
        {

            lock (_functionsFactoryByKey)
            {
                int key = GetFunctionsManagerKey(functionManager);
                if (_functionsFactoryByKey.ContainsKey(key))
                {
                    functionManager.Dispose();
                    _functionsFactoryByKey.Remove(key);
                }
            }
        }

        internal static FunctionParameter GetTask(string componentName, string stateMachineName)
        {
            int key = GetFunctionsManagerKey(componentName, stateMachineName);
            lock (_functionsFactoryByKey)
            {
                if (_functionsFactoryByKey.ContainsKey(key))
                {
                    return _functionsFactoryByKey[key].GetTask();
                }
            }

            return null;
        }

        internal static void AddTaskResult(FunctionResult result)
        {
            int key = GetFunctionsManagerKey(result.ComponentName, result.StateMachineName);
            lock (_functionsFactoryByKey)
            {
                if (_functionsFactoryByKey.ContainsKey(key))
                {
                    _functionsFactoryByKey[key].AddTaskResult(result);
                }
            }

        }

        internal static int GetFunctionsManagerKey(string componentName, string stateMachineName)
        {
            if (string.IsNullOrEmpty(componentName) || string.IsNullOrEmpty(stateMachineName))
            {
                return -1;
            }
            return componentName.GetHashCode() ^ stateMachineName.GetHashCode();
        }
        internal static int GetFunctionsManagerKey(FunctionsManager functionsManager)
        {
            return GetFunctionsManagerKey(functionsManager.ComponentName, functionsManager.StateMachineName);
        }
    }
}
