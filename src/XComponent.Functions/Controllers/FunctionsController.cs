using System.Web.Http;
using XComponent.Functions.Core;

namespace XComponent.Functions.Controllers
{
    public class FunctionsController: ApiController
    {
        public FunctionParameter GetTask(string componentName, string stateMachineName)
        {
            return FunctionsFactory.GetTask(componentName, stateMachineName);
        }

        public void PostTaskResult(FunctionResult result)
        {
            FunctionsFactory.AddTaskResult(result);
        }

    }
}
