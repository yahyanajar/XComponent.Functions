using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace XComponent.Functions.Core
{
    public interface IFunctionsManager : IDisposable {
        string ComponentName { get;  }
        string StateMachineName { get;  }

        Task AddTask(object xcEvent, object publicMember, object internalMember, object context, object sender, [CallerMemberName] string functionName = null);
        FunctionParameter GetTask();
        void AddTaskResult(FunctionResult functionResult);
    }
}
