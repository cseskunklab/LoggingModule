using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using LogModule.Core;

namespace LoggingModule
{
    public static class DirectMethods
    {
        private static IRemote _innerService;

        public static void Init(IRemote innerService)
        {
            _innerService = innerService;

            ConnectToIoTHub();
        }

        private static async void ConnectToIoTHub()
        {
            AmqpTransportSettings amqpSetting = new AmqpTransportSettings(TransportType.Amqp_Tcp_Only);
            ITransportSettings[] settings = { amqpSetting };
            
            // Open a connection to the Edge runtime
            var ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            //var ioTHubModuleClient = ModuleClient.CreateFromConnectionString("xxx");

            await ioTHubModuleClient.OpenAsync();

            Console.WriteLine("IoT Hub module client initialized.");

            await ioTHubModuleClient.SetMethodHandlerAsync(nameof(Echo), Echo, ioTHubModuleClient);

            await ioTHubModuleClient.SetMethodHandlerAsync(nameof(UploadFile), UploadFile, ioTHubModuleClient);
            await ioTHubModuleClient.SetMethodHandlerAsync(nameof(DownloadFile), DownloadFile, ioTHubModuleClient);
            await ioTHubModuleClient.SetMethodHandlerAsync(nameof(TruncateFile), TruncateFile, ioTHubModuleClient);
        }

        //Direct method
        public static Task<MethodResponse> Echo(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine("Direct method called with " + methodRequest.DataAsJson);

            if (_innerService == null)
            {
                throw new Exception("Direct Methods have not been initialized");
            }

            // Acknowlege the direct method call with a 200 success message
            string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        public static Task<MethodResponse> UploadFile(MethodRequest methodRequest, object userContext)
        {
            //Task UploadFile(string sourcePath, string sourceFilename, string containerName, string targetFilename, string contentType, bool append = false);
            //Task UploadFile(string sourcePath, string sourceFilename, string sasUri, string contentType, bool append = false);

            try
            {
                dynamic data = JsonConvert.DeserializeObject(methodRequest.DataAsJson);

                if (data.GetType().GetProperty("containerName") != null)
                {
                    _innerService.UploadFile(data.sourcePath, data.sourceFilename, data.containerName, data.targetFilename,
                                    data.contentType, data.append).Wait();
                }
                else
                {
                    _innerService.UploadFile(data.sourcePath, data.sourceFilename, data.sasUri,
                                    data.contentType, data.append).Wait();
                }

                return Task.FromResult(new MethodResponse(200));
            }
            catch
            {
                return Task.FromResult(new MethodResponse(500));
            }
        }

        public static Task<MethodResponse> DownloadFile(MethodRequest methodRequest, object userContext)
        {
            //Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append = false);
            //Task DownloadFile(string targetPath, string targetFilename, string sasUri, bool append = false);

            try
            {
                dynamic data = JsonConvert.DeserializeObject(methodRequest.DataAsJson);

                if (data.GetType().GetProperty("containerName") != null)
                {
                    _innerService.UploadFile(data.targetPath, data.targetFilename, data.containerName, data.filename, data.append).Wait();
                }
                else
                {
                    _innerService.UploadFile(data.targetPath, data.targetFilename, data.sasUri, data.append).Wait();
                }

                return Task.FromResult(new MethodResponse(200));
            }
            catch
            {
                return Task.FromResult(new MethodResponse(500));
            }
        }

        public static Task<MethodResponse> TruncateFile(MethodRequest methodRequest, object userContext)
        {
            //Task TruncateFile(string sourcePath, string sourceFilename, int maxBytes);

            try
            {
                dynamic data = JsonConvert.DeserializeObject(methodRequest.DataAsJson);

                _innerService.TruncateFile(data.sourcePath, data.sourceFilename, data.maxBytes).Wait();

                return Task.FromResult(new MethodResponse(200));
            }
            catch
            {
                return Task.FromResult(new MethodResponse(500));
            }
        }



    }
}
