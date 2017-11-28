using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using XComponent.Functions.Core;

namespace XComponent.Functions.Controllers
{
    public class FunctionsController: ApiController
    {
        [SwaggerResponse(HttpStatusCode.OK, "Next available task", typeof(FunctionParameter))]
        [SwaggerResponse(HttpStatusCode.NoContent, "No task available")]
        public HttpResponseMessage GetTask(string componentName, string stateMachineName)
        {
            var response = FunctionsFactory.Instance.GetTask(componentName, stateMachineName);
            return Request.CreateResponse<FunctionParameter>(response == null ? HttpStatusCode.NoContent : HttpStatusCode.OK, response);
        }

        public void PostTaskResult(FunctionResult result)
        {
            FunctionsFactory.Instance.AddTaskResult(result);
        }

    }
}
