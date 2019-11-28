using Learun.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Learun.Application.Web.API
{
    [RoutePrefix("rest")]
    public class HomeController : BaseApi
    {
        [Route("test")]
        [ApiAuthorize(FilterMode.Ignore)]
        public ResParameter Get()
        {
            return Success("成功");
        }
    }
}
