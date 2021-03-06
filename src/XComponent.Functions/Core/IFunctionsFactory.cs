using System;

namespace XComponent.Functions.Core
{
    public interface IFunctionsFactory {
        IFunctionsManager CreateFunctionsManager(string componentName, string stateMachineName, Uri url = null);
        FunctionParameter GetTask(string componentName, string stateMachineName);
        void AddTaskResult(FunctionResult result);
    }
}
