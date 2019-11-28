using System.Net.Http.Formatting;
using System.Web.Http;

namespace Learun.Application.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            //默认返回 json
            GlobalConfiguration.Configuration.Formatters
                .JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "json", "application/json"));
            //json 序列化设置
            //统一设置Json中的日期格式    
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.
                SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            //字段为null的处理成空字符
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter
            //    .SerializerSettings.ContractResolver = new NullToEmptyStringResolver();

            // Web API 路由
            config.MapHttpAttributeRoutes();
            //跨域设置
            config.EnableCors(new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*"));
            //解决多个POST的问题
            config.Routes.MapHttpRoute(
              name: "DefaultApi",
              routeTemplate: "rest/{controller}/{action}/{id}",
              defaults: new { id = RouteParameter.Optional }
          );

        }
    }
}
