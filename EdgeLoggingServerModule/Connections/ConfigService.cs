using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VirtualRtu.Common.Configuration;

namespace EdgeLoggingServerModule.Connections
{
    public class ConfigService
    {

        public event System.EventHandler<ModuleConfigurationEventArgs> OnModuleConfig;
        private CancellationTokenSource source;
        private EdgeMqttClient client;
        private ModuleClient moduleClient;

        public async Task InitAsync()
        {
            Console.WriteLine("{0} Staring configuration", DateTime.Now.ToString("hh:MM:ss.ffff"));
            try
            {
                if (moduleClient != null)
                {
                    await moduleClient.CloseAsync().ConfigureAwait(false);
                    moduleClient = null;
                }

                Console.WriteLine("{0} Initializing module client", DateTime.Now.ToString("hh:MM:ss.ffff"));
                moduleClient = await ModuleClient.CreateFromEnvironmentAsync(TransportType.Mqtt);
                //moduleClient = ModuleClient.CreateFromConnectionString("HostName=virtualrtu.azure-devices.net;DeviceId=edgedevice1;ModuleId=fieldgateway;SharedAccessKey=7xQe9eE6ep7TrHvDWiKWGTlEaGYVLBLCS9NxVVHTYt8=", TransportType.Mqtt);

                Console.WriteLine("{0} Module client created.", DateTime.Now.ToString("hh:MM:ss.ffff"));
                var context = moduleClient;

                Console.WriteLine("{0} Opening module client explcitly", DateTime.Now.ToString("hh:MM:ss.ffff"));
                await moduleClient.OpenAsync().ConfigureAwait(false);

                Console.WriteLine("{0} Module client explictly opened", DateTime.Now.ToString("hh:MM:ss.ffff"));

                

                Console.WriteLine("{0} Getting twin", DateTime.Now.ToString("hh:MM:ss.ffff"));
                //get the twin and check for configuration
                Twin twin = await moduleClient.GetTwinAsync().ConfigureAwait(false);

                Console.WriteLine("{0} Twin open", DateTime.Now.ToString("hh:MM:ss.ffff"));
                Console.WriteLine("{0} Getting twin collection", DateTime.Now.ToString("hh:MM:ss.ffff"));
                TwinCollection collection = twin.Properties.Desired;
                Console.WriteLine("{0} Reading luss and service url", DateTime.Now.ToString("hh:MM:ss.ffff"));
                if (collection.Contains("luss") && collection.Contains("serviceUrl"))
                {
                    string luss = collection["luss"];
                    string serviceUrl = collection["serviceUrl"];

                    Console.WriteLine("{0} Twin properites LUSS = {1}  Service URL = {2}", DateTime.Now.ToString("hh:MM:ss.ffff"), luss, serviceUrl);
                    if (!string.IsNullOrEmpty(luss) && !string.IsNullOrEmpty(serviceUrl))
                    {
                        Console.WriteLine("{0} Processing configuration", DateTime.Now.ToString("hh:MM:ss.ffff"));
                        await ProcessTwinConfig(luss, serviceUrl);
                        Console.WriteLine("{0} Configuration processing complete", DateTime.Now.ToString("hh:MM:ss.ffff"));
                    }
                }
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("{0} Fault at start - AG {1}", DateTime.Now.ToString("hh:MM:ss.ffff"), ae.Flatten().InnerException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Fault at start - {1}", DateTime.Now.ToString("hh:MM:ss.ffff"), ex.Message);
            }
        }


        private async Task ProcessTwinConfig(string luss, string serviceUrl)
        {
            try
            {
                IssuedConfig issuedConfig = null;

                string requestUrl = String.Format("{0}&luss={1}", serviceUrl, luss);
                Console.WriteLine("Service URL = {0}", requestUrl);

                Console.WriteLine("{0} Making Azure function call", DateTime.Now.ToString("hh:MM:ss.ffff"));
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage message = await httpClient.GetAsync(requestUrl);
                if (message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string jsonString = await message.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        issuedConfig = JsonConvert.DeserializeObject<IssuedConfig>(jsonString);
                        //write to volume
                        Console.WriteLine("Attempting to file stream");
                        //using (var stream = new FileStream("./data/config.json", FileMode.Create))
                        //{                            
                        //    byte[] buffer = Encoding.UTF8.GetBytes(jsonString);
                        //    await stream.WriteAsync(buffer, 0, buffer.Length);
                        //    stream.Flush();
                        //    stream.Close();
                        //    Console.WriteLine("File stream written");
                        //}

                        OnModuleConfig?.Invoke(this, new ModuleConfigurationEventArgs(issuedConfig));
                    }
                }
                else
                {
                    Console.WriteLine("{0} Request for configuration return HTTP status '{1}'", DateTime.Now.ToString("hh:MM:ss.ffff"), message.StatusCode);
                    OnModuleConfig?.Invoke(this, new ModuleConfigurationEventArgs(null));
                }
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("Desired properties for module update error - AG.");
                Console.WriteLine(ae.Flatten().InnerException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Desired properties for module update error.");
                Console.WriteLine(ex.Message);
            }
        }



    }
}
