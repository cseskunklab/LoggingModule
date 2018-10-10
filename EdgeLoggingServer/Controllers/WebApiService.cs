using Microsoft.AspNetCore.Mvc;

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
