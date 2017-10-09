using XComponent.Functions.Core;

namespace XComponent.Functions.Utilities
{
    internal static class FunctionParameterFactory
    {
        public static FunctionParameter CreateFunctionParameter(object xcEvent, object publicMember, object internalMember,
            object context, string componentName, string stateMachineName, string functionName)
        {
            FunctionParameter functionParameter =
                new FunctionParameter
                {
                    Event = xcEvent,
                    PublicMember = publicMember,
                    InternalMember = internalMember,
                    Context = context,
                    ComponentName = componentName,
                    StateMachineName = stateMachineName,
                    FunctionName = functionName
                };

            return functionParameter;
        }
    }
}
