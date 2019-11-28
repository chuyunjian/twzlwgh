using System.Web.Mvc;

namespace Learun.Application.Web.Areas.SYS_Code
{
    public class SYS_CodeAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SYS_Code";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SYS_Code_default",
                "SYS_Code/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}