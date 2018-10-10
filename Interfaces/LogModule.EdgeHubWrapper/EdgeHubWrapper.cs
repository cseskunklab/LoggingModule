using Microsoft.Azure.Devices.Client;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogModule
{
    public static class EdgeHubWrapper
    {
        static DirectMethodInterface.DirectMethodInterface directMethodInterface = null;
        const string storageAccountName = "loggingmodulestore";
        const string storageAccountKey = "PEY5ADgPE4Fg57f17jJyVRQJvbNU9CCrc2cf0x/8JfoNVDbuqZkkDr0A9Os2X27FNFfzv3dAirSra5pOBkzjEw==";

        public static async Task Init()
        {
            directMethodInterface = new DirectMethodInterface.DirectMethodInterface(storageAccountName, storageAccountKey);
            AmqpTransportSettings amqpSetting = new AmqpTransportSettings(TransportType.Amqp_Tcp_Only);
            ITransportSettings[] settings = { amqpSetting };

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync().ConfigureAwait(false);
            Console.WriteLine("IoT Hub module client initialized.");

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("UploadFile", UploadFileMessage, ioTHubModuleClient);
            await ioTHubModuleClient.SetInputMessageHandlerAsync("DownloadFile", DownloadFileMessage, ioTHubModuleClient);
        }

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>

        static async Task<MessageResponse> UploadFileMessage(Message message, object userContext)
        {
            //For test purpose only
            message.Properties.Add("sourcePath", @"C:\");
            message.Properties.Add("sourceFilename", @"Test.txt");
            message.Properties.Add("containerName", @"data");
            message.Properties.Add("targetFilename", @"Test.txt");
            message.Properties.Add("contentType", @"text/plain");
            message.Properties.Add("append", @"False");
            //*********************

            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine($"Received message body in 'UploadFile' method: [{messageString}]");
            Console.WriteLine($"Message properties:");
            foreach (string key in message.Properties.Keys)
            {
                Console.WriteLine($"\tKey:{key}, Value: {message.Properties[key]}");
            }

            // Process code here
            //Read File Content
            string fullFilePath = Path.Combine(message.Properties["sourcePath"], message.Properties["sourceFilename"]);
            Console.WriteLine($"Source file location: {fullFilePath}");

            //Check SaS Uri property
            string sasUri = null;
            message.Properties.TryGetValue("sasUri", out sasUri);

            if(string.IsNullOrEmpty(sasUri))
            {
                try
                {
                    await directMethodInterface.UploadFile(message.Properties["sourcePath"], message.Properties["sourceFilename"], message.Properties["containerName"], message.Properties["targetFilename"], message.Properties["contentType"], bool.Parse(message.Properties["append"]));
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error uploading file to storage: {ex.Message}");
                }
            }

            return MessageResponse.Completed;
        }

        static async Task<MessageResponse> DownloadFileMessage(Message message, object userContext)
        {
            //For test purpose only
            message.Properties.Add("targetPath", @"C:/Users");
            message.Properties.Add("targetFilename", @"Test.txt");
            message.Properties.Add("containerName", @"data");
            message.Properties.Add("filename", @"Test.txt");
            message.Properties.Add("append", @"False");
            //*********************

            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine($"Received message body in 'DownloadFile' method: [{messageString}]");

            // Process code here
            //Check SaS Uri property
            string sasUri = null;
            message.Properties.TryGetValue("sasUri", out sasUri);

            if (string.IsNullOrEmpty(sasUri))
            {
                try
                {
                    await directMethodInterface.DownloadFile(message.Properties["targetPath"], message.Properties["targetFilename"], message.Properties["containerName"], message.Properties["filename"], bool.Parse(message.Properties["append"]));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading file from storage: {ex.Message}");
                }
            }

            return MessageResponse.Completed;
        }

        public static string GetJsonTokenValue(string json)
        {
            return string.Empty;
        }
    }
}
