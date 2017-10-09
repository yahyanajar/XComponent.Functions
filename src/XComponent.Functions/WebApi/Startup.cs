using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;

namespace XComponent.Functions.WebApi
{
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var webApiConfiguration = ConfigureWebApi();
            app.UseWebApi(webApiConfiguration);
        }

        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();
           
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            config.EnableSwagger(
                delegate (SwaggerDocsConfig c)
                {
                    c.RootUrl(req => req.RequestUri.GetLeftPart(UriPartial.Authority) + config.VirtualPathRoot.TrimEnd('/'));

                    c.SingleApiVersion("v1", "XComponent Functions  API");
                }
            ).EnableSwaggerUi();

            //only keep JSON format
            var jqueryFormatter = config.Formatters.FirstOrDefault(x => x.GetType() == typeof(JQueryMvcFormUrlEncodedFormatter));
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Remove(config.Formatters.FormUrlEncodedFormatter);
            config.Formatters.Remove(jqueryFormatter);

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true };

            return config;
        }
    }
}
