using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdgeLoggingServer.Controllers
{
    public abstract class WebApiService<IService> : ControllerBase
    {
        protected IService InnerService
        {
            get;
            private set;
        }

        public WebApiService(IService service)
        {
            InnerService = service;
        }
    }
}
