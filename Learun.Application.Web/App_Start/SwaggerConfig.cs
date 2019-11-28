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
                        //����
                        c.SingleApiVersion("v1", "API���߲���");
                        //�������ã�

                        c.IncludeXmlComments(string.Format("{0}/bin/Learun.Application.Web.XML", System.AppDomain.CurrentDomain.BaseDirectory));
                        //���XML��model�������XML����model���ע��
                        c.IncludeXmlComments(string.Format("{0}/bin/Learun.Application.TwoDevelopment.XML", System.AppDomain.CurrentDomain.BaseDirectory));
                        // ͬ���Ľӿ��� �����˲�ͬ������
                        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                        c.CustomProvider((defaultProvider) => new SwaggerCacheProvider(defaultProvider, string.Format("{0}/bin/Learun.Application.Web.XML", System.AppDomain.CurrentDomain.BaseDirectory)));
                        //������Զ����filter���жϷ����Ƿ����ͷ��tokenʹ��
                        c.OperationFilter<HttpAuthHeaderFilter>();
                    })
                .EnableSwaggerUi(c =>
                    {

                        c.InjectJavaScript(thisAssembly, "Learun.Application.Web.Content.SwaggerUi.Swagger-china.js");

                    });
        }
    }
}
