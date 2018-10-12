using LogModule.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

namespace EdgeLoggingServer
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o =>
            {
                o.InputFormatters.Insert(0, new BinaryInputFormatter());
                o.OutputFormatters.Insert(0, new BinaryOutputFormatter());
            });


            string connectionString = _configuration["connectionString"];

            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            //services.AddTransient<ILocal>(s => new LocalFileIO(storageAccount.Credentials.AccountName, storageAccount.Credentials.SASToken));
            services.AddTransient<ILocal>(s => new LocalFileIO(connectionString));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
       
        }
    }
}
