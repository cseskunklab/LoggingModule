using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdgeLoggingServer
{
    public class EdgeLoggingServer
    {
        private IWebHost _webHost;

        
        public EdgeLoggingServer(int port, string storageConnectionString)
        {
            _webHost = CreateWebHostBuilder(port, storageConnectionString).Build();
        }

        public async Task RunAsync()
        {
             await _webHost.RunAsync();
        }

        public async Task StartAsync()
        {
            await _webHost.StartAsync();
        }

        public async Task StopAsync(string[] args)
        {
            await _webHost.StopAsync();
        }

        private IWebHostBuilder CreateWebHostBuilder(int port, string storageConnectionString)
        {
            return WebHost.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var configKeys = new Dictionary<string, string>() { { "connectionString", storageConnectionString } };
                    config.AddInMemoryCollection(configKeys);
                })
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
                   options.ListenAnyIP(port);

               });
        }
    }
}
