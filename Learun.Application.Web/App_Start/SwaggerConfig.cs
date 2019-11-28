using System.Web.Http;
using WebActivatorEx;
using Learun.Application.Web;
using Swashbuckle.Application;
using System.Linq;
using Learun.Application.Web.Swagger;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Learun.Application.Web
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        //名称
                        c.SingleApiVersion("v1", "API在线测试");
                        //配置启用：

                        c.IncludeXmlComments(string.Format("{0}/bin/Learun.Application.Web.XML", System.AppDomain.CurrentDomain.BaseDirectory));
                        //这个XML是model层输出的XML包含model层的注释
                        c.IncludeXmlComments(string.Format("{0}/bin/Learun.Application.TwoDevelopment.XML", System.AppDomain.CurrentDomain.BaseDirectory));
                        // 同样的接口名 传递了不同参数，
                        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                        c.CustomProvider((defaultProvider) => new SwaggerCacheProvider(defaultProvider, string.Format("{0}/bin/Learun.Application.Web.XML", System.AppDomain.CurrentDomain.BaseDirectory)));
                        //这个是自定义的filter，判断方法是否添加头部token使用
                        c.OperationFilter<HttpAuthHeaderFilter>();
                    })
                .EnableSwaggerUi(c =>
                    {

                        c.InjectJavaScript(thisAssembly, "Learun.Application.Web.Content.SwaggerUi.Swagger-china.js");

                    });
        }
    }
}
