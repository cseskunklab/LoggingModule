using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;
using System.Threading.Tasks;

namespace EdgeLoggingServer
{
    public class EdgeLoggingServer
    {
        private static IWebHost _webHost;

        public static async Task RunAsync(string[] args)
        {
            _webHost = CreateWebHostBuilder(args).Build();
           
            await _webHost.RunAsync();
        }

        public static async Task StopAsync(string[] args)
        {
            await _webHost.StopAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>()
               .UseKestrel(options =>
               {
                   options.Limits.MaxConcurrentConnections = 100;
                   options.Limits.MaxConcurrentUpgradedConnections = 100;
                   options.Limits.MaxRequestBodySize = 10 * 1024;
                   options.Limits.MinRequestBodyDataRate =
                       new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                   options.Limits.MinResponseDataRate =
                       new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                   options.ListenAnyIP(8080);

               });
    }
}
