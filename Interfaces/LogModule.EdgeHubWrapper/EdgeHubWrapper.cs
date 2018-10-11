using LogModule.Core;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogModuleWrapper
{
    public static class EdgeHubWrapper
    {
        static RemoteFileIO remoteFileIO = null;
        static LocalFileIO localFileIO = null;

        public static async Task Init(string storageAccountName, string storageAccountKey)
        {
            remoteFileIO = new RemoteFileIO(storageAccountName, storageAccountKey);
            localFileIO = new LocalFileIO(storageAccountName, storageAccountKey);

            AmqpTransportSettings amqpSetting = new AmqpTransportSettings(TransportType.Amqp_Tcp_Only);
            ITransportSettings[] settings = { amqpSetting };

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync().ConfigureAwait(false);

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("UploadFile", UploadFileMessage, ioTHubModuleClient);
            await ioTHubModuleClient.SetInputMessageHandlerAsync("DownloadFile", DownloadFileMessage, ioTHubModuleClient);

            await ioTHubModuleClient.SetInputMessageHandlerAsync("RemoveFile", RemoveFileMessage, ioTHubModuleClient);
            await ioTHubModuleClient.SetInputMessageHandlerAsync("WriteFile", WriteFileMessage, ioTHubModuleClient);
            await ioTHubModuleClient.SetInputMessageHandlerAsync("GetFile", GetFileMessage, ioTHubModuleClient);
            await ioTHubModuleClient.SetInputMessageHandlerAsync("ListFiles", ListFilesMessage, ioTHubModuleClient);

            Console.WriteLine("EdgeHub logging module has been initialized");
        }

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>

        //Remote operations
        static async Task<MessageResponse> UploadFileMessage(Message message, object userContext)
        {
            //For test purpose only
            //message.Properties.Add("sourcePath", @"C:\");
            //message.Properties.Add("sourceFilename", @"Test.txt");
            //message.Properties.Add("containerName", @"data");
            //message.Properties.Add("targetFilename", @"Test.txt");
            //message.Properties.Add("contentType", @"text/plain");
            //message.Properties.Add("append", @"False");
            //*********************

            logReceivedMessage(message);

            #region Checking required properties
            if (!message.Properties.ContainsKey("sourcePath"))
            {
                Console.WriteLine("A required 'sourcePath' property is missed.");
                return MessageResponse.Abandoned;
            }
            if (!message.Properties.ContainsKey("sourceFilename"))
            {
                Console.WriteLine("A required 'sourceFilename' property is missed.");
                return MessageResponse.Abandoned;
            }
            if (!message.Properties.ContainsKey("containerName"))
            {
                Console.WriteLine("A required 'containerName' property is missed.");
                return MessageResponse.Abandoned;
            }
            if (!message.Properties.ContainsKey("targetFilename"))
            {
                Console.WriteLine("A required 'targetFilename' property is missed.");
                return MessageResponse.Abandoned;
            }

            //Resolving values
            if (!message.Properties.ContainsKey("contentType")) message.Properties.Add("contentType", @"application/octet-stream");
            if (!message.Properties.ContainsKey("append")) message.Properties.Add("append", @"False");
            else
            {
                bool append = false;
                if(!bool.TryParse(message.Properties["append"], out append))
                {
                    Console.WriteLine($"Failed to convert 'append' value to boolean. Value is {message.Properties["append"]}");
                    return MessageResponse.Abandoned;
                }
            }
            #endregion

            // Process code here
            //Read File Content
            string fullFilePath = Path.Combine(message.Properties["sourcePath"], message.Properties["sourceFilename"]);
            Console.WriteLine($"Source file location: {fullFilePath}");

            //Check SaS Uri property
            string sasUri = null;
            message.Properties.TryGetValue("sasUri", out sasUri);

            if (string.IsNullOrEmpty(sasUri))
            {
                try
                {
                    await remoteFileIO.UploadFile(message.Properties["sourcePath"], message.Properties["sourceFilename"], message.Properties["containerName"], message.Properties["targetFilename"], message.Properties["contentType"], bool.Parse(message.Properties["append"]));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error uploading file to storage: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    await remoteFileIO.UploadFile(message.Properties["sourcePath"], message.Properties["sourceFilename"], message.Properties["sasUri"], message.Properties["contentType"], bool.Parse(message.Properties["append"]));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error uploading file to storage: {ex.Message}");
                }
            }

            Console.WriteLine("Done!");

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

            logReceivedMessage(message);

            #region Checking required properties
            if (!message.Properties.ContainsKey("targetPath"))
            {
                Console.WriteLine("A required 'targetPath' property is missed.");
                return MessageResponse.Abandoned;
            }
            if (!message.Properties.ContainsKey("targetFilename"))
            {
                Console.WriteLine("A required 'targetFilename' property is missed.");
                return MessageResponse.Abandoned;
            }
            if (!message.Properties.ContainsKey("containerName"))
            {
                Console.WriteLine("A required 'containerName' property is missed.");
                return MessageResponse.Abandoned;
            }
            if (!message.Properties.ContainsKey("filename"))
            {
                Console.WriteLine("A required 'filename' property is missed.");
                return MessageResponse.Abandoned;
            }

            //Resolving values
            if (!message.Properties.ContainsKey("append")) message.Properties.Add("append", @"False");
            else
            {
                bool append = false;
                if (!bool.TryParse(message.Properties["append"], out append))
                {
                    Console.WriteLine($"Failed to convert 'append' value to boolean. Value is {message.Properties["append"]}");
                    return MessageResponse.Abandoned;
                }
            }
            #endregion

            // Process code here
            try
            {
                await remoteFileIO.DownloadFile(message.Properties["targetPath"], message.Properties["targetFilename"], message.Properties["containerName"], message.Properties["filename"], bool.Parse(message.Properties["append"]));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file from storage: {ex.Message}");
            }

            Console.WriteLine("Done!");

            return MessageResponse.Completed;
        }

        //Local Operations
        static async Task<MessageResponse> RemoveFileMessage(Message message, object userContext)
        {
            //For test purpose only
            message.Properties.Add("sourcePath", @"C:/Users");
            message.Properties.Add("sourceFilename", @"Test.txt");
            //*********************

            logReceivedMessage(message);

            #region Checking required properties
            if (!message.Properties.ContainsKey("sourcePath"))
            {
                Console.WriteLine("A required 'sourcePath' property is missed.");
                return MessageResponse.Abandoned;
            }
            if (!message.Properties.ContainsKey("sourceFilename"))
            {
                Console.WriteLine("A required 'sourceFilename' property is missed.");
                return MessageResponse.Abandoned;
            }
            #endregion

            // Process code here
            try
            {
                await localFileIO.RemoveFile(message.Properties["sourcePath"], message.Properties["sourceFilename"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting local file: {ex.Message}");
            }

            Console.WriteLine("Done!");

            return MessageResponse.Completed;
        }
        static async Task<MessageResponse> WriteFileMessage(Message message, object userContext)
        {
            //For test purpose only
            message.Properties.Add("sourcePath", @"C:/Users");
            message.Properties.Add("sourceFilename", @"Test.txt");
            message.Properties.Add("append", @"False");
            //*********************

            logReceivedMessage(message);

            #region Checking required properties
            if (!message.Properties.ContainsKey("sourcePath"))
            {
                Console.WriteLine("A required 'sourcePath' property is missed.");
                return MessageResponse.Abandoned;
            }
            if (!message.Properties.ContainsKey("sourceFilename"))
            {
                Console.WriteLine("A required 'sourceFilename' property is missed.");
                return MessageResponse.Abandoned;
            }

            //Resolving values
            if (!message.Properties.ContainsKey("append")) message.Properties.Add("append", @"False");
            else
            {
                bool append = false;
                if (!bool.TryParse(message.Properties["append"], out append))
                {
                    Console.WriteLine($"Failed to convert 'append' value to boolean. Value is {message.Properties["append"]}");
                    return MessageResponse.Abandoned;
                }
            }
            #endregion

            // Process code here
            try
            {
                await localFileIO.WriteFile(message.Properties["sourcePath"], message.Properties["sourceFilename"], message.GetBytes(), bool.Parse(message.Properties["append"]));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing local file: {ex.Message}");
            }

            Console.WriteLine("Done!");

            return MessageResponse.Completed;
        }
        static async Task<MessageResponse> GetFileMessage(Message message, object userContext)
        {
            //For test purpose only
            message.Properties.Add("sourcePath", @"C:/Users");
            message.Properties.Add("sourceFilename", @"Test.txt");
            //*********************

            logReceivedMessage(message);

            #region Checking required properties
            if (!message.Properties.ContainsKey("sourcePath"))
            {
                Console.WriteLine("A required 'sourcePath' property is missed.");
                return MessageResponse.Abandoned;
            }
            if (!message.Properties.ContainsKey("sourceFilename"))
            {
                Console.WriteLine("A required 'sourceFilename' property is missed.");
                return MessageResponse.Abandoned;
            }
            #endregion

            // Process code here
            try
            {
                byte[] content = await localFileIO.GetFile(message.Properties["sourcePath"], message.Properties["sourceFilename"]);

                var moduleClient = userContext as ModuleClient;
                if (moduleClient == null)
                {
                    throw new InvalidOperationException("UserContext doesn't exist. Not able to write in output.");
                }
                else
                {
                    await moduleClient.SendEventAsync("GetFileOutput", new Message(content));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting local file: {ex.Message}");
            }

            Console.WriteLine("Done!");

            return MessageResponse.Completed;
        }
        static async Task<MessageResponse> ListFilesMessage(Message message, object userContext)
        {
            //For test purpose only
            message.Properties.Add("sourcePath", @"C:/Users");
            //*********************

            logReceivedMessage(message);

            #region Checking required properties
            if (!message.Properties.ContainsKey("sourcePath"))
            {
                Console.WriteLine("A required 'sourcePath' property is missed.");
                return MessageResponse.Abandoned;
            }
            #endregion

            // Process code here
            try
            {
                string[] content = await localFileIO.ListFiles(message.Properties["sourcePath"], null);

                var moduleClient = userContext as ModuleClient;
                if (moduleClient == null)
                {
                    throw new InvalidOperationException("UserContext doesn't exist. Not able to write in output.");
                }
                else
                {
                    await moduleClient.SendEventAsync("ListFilesOutput", new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content))));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting local file: {ex.Message}");
            }

            Console.WriteLine("Done!");

            return MessageResponse.Completed;
        }

        //static async Task RefreshDesiredProperties(ModuleClient ioTHubModuleClient)
        //{
        //    // Read the TemperatureThreshold value from the module twin's desired properties
        //    var moduleTwin = await ioTHubModuleClient.GetTwinAsync();
        //    var moduleTwinCollection = moduleTwin.Properties.Desired;

        //    try
        //    {
        //        storageAccountName = moduleTwinCollection["storageAccountName"];
        //    }
        //    catch (ArgumentOutOfRangeException e)
        //    {
        //        Console.WriteLine($"Property storageAccountName not exist: {e.Message}");
        //    }

        //    try
        //    {
        //        storageAccountKey = moduleTwinCollection["storageAccountKey"];
        //    }
        //    catch (ArgumentOutOfRangeException e)
        //    {
        //        Console.WriteLine($"Property storageAccountKey not exist: {e.Message}");
        //    }

        //    // Attach a callback for updates to the module twin's desired properties.
        //    await ioTHubModuleClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertiesUpdate, null);
        //}

        //static Task OnDesiredPropertiesUpdate(TwinCollection desiredProperties, object userContext)
        //{
        //    try
        //    {
        //        Console.WriteLine("Desired property change:");
        //        Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));

        //        if (desiredProperties["storageAccountName"] != null) storageAccountName = desiredProperties["storageAccountName"];
        //        if (desiredProperties["storageAccountKey"] != null) storageAccountKey = desiredProperties["storageAccountKey"];

        //    }
        //    catch (AggregateException ex)
        //    {
        //        foreach (Exception exception in ex.InnerExceptions)
        //        {
        //            Console.WriteLine();
        //            Console.WriteLine("Error when receiving desired property: {0}", exception);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine();
        //        Console.WriteLine("Error when receiving desired property: {0}", ex.Message);
        //    }
        //    return Task.CompletedTask;
        //}
        static void logReceivedMessage(Message message)
        {
            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine($"Received message body in 'UploadFile' method: [{messageString}], size: {messageBytes.LongLength} bytes");
            Console.WriteLine($"Message properties:");
            foreach (string key in message.Properties.Keys)
            {
                Console.WriteLine($"\tKey:{key}, Value: {message.Properties[key]}");
            }
        }
    }
}
