using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FieldGatewayMicroservice.Connections;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SkunkLab.VirtualRtu.ModBus;
using VirtualRtu.Common.Configuration;

namespace FieldGatewayMicroservice
{
    public class Program
    {

        private static ConfigService configService;
        private static IssuedConfig config;
        //private static string url;

        public static void Main(string[] args)
        {
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("------------Starting Field Gateway ----------");
            Console.WriteLine("---------------------------------------------");


            // make sure Unobserved Exceptions don't kill module - Catch and signal event
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //string ip = GetAddress();
            //url = "http://0.0.0.0:8888/";

            configService = new ConfigService();
            configService.OnModuleConfig += ConfigService_OnModuleConfig;
            Task task = configService.InitAsync();
            Task.WhenAll(task);

            CreateWebHostBuilder(args).Build().Run();
        }

        private static void ConfigService_OnModuleConfig(object sender, ModuleConfigurationEventArgs e)
        {
            if (e.Config != null)
            {
                Console.WriteLine("Received config and will attempt to start mqtt client");
                config = e.Config;               
            }
            else
            {
                byte[] buffer = File.ReadAllBytes("./data/config.json");
                string jsonString = Encoding.UTF8.GetString(buffer);
                config = JsonConvert.DeserializeObject<IssuedConfig>(jsonString);
            }

            Task task = EdgeClient.Init(config);
            Task.WhenAll(task);

        }       

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();

            Console.WriteLine("********** Unobserved Exception Block **********");
            Console.WriteLine("Error = '{0}'", e.Exception.Message);

            Exception inner = e.Exception.InnerException;
            int indexer = 0;
            while (inner != null)
            {
                indexer++;
                Console.WriteLine("Inner index {0} '{1}'", indexer, inner.Message);
                if (String.IsNullOrEmpty(inner.Message))
                {
                    Console.WriteLine("-------------- Start Stack Trace {0} ---------------", indexer);
                    Console.WriteLine(inner.StackTrace);
                    Console.WriteLine("-------------- End Stack Trace {0} ---------------", indexer);
                }
                inner = inner.InnerException;
            }

            Console.WriteLine("********** End Unobserved Exception Block **********");
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
                    options.ListenAnyIP(8888);

                });

        public static string GetAddress()
        {
            string requestUrl = null;
            IPHostEntry entry = Dns.GetHostEntry("mbpa");
            string ipAddressString = null;

            foreach (var address in entry.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    if (address.ToString().Contains("172"))
                    {
                        ipAddressString = address.ToString();
                        break;
                    }

                }
            }

            if (ipAddressString != null)
            {
                requestUrl = String.Format("http://{0}:8889/api/rtuinput", ipAddressString);
                Console.WriteLine("REQUEST URL = '{0}'", requestUrl);
            }
            else
            {
                Console.WriteLine("NO IP ADDRESS FOUND");
            }

            return requestUrl;
        }


    }
}
