using LogModule.Core;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
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

            // Open a connection to the Edge runtime
            ModuleClient ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(TransportType.Mqtt);
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

            if (!validateStringParams(message, "sourcePath", "sourceFilename", "targetFilename")) return MessageResponse.Abandoned;
            if(!validateStringParams(message, "containerName"))
            {
                if(!validateStringParams(message, "sasUri"))
                {
                    Console.WriteLine("'containerName' or 'sasUri' should be set.");
                    return MessageResponse.Abandoned;
                }
            }

            //Resolving values
            if (!validateStringParams(message, "contentType"))
            {
                message.Properties.Add("contentType", @"application/octet-stream");
                Console.WriteLine("'contentType' property missed. 'contentType' set to 'application/octet-stream'.");
            }

            if (!validateStringParams(message, "append"))
            {
                message.Properties.Add("append", @"False");
                Console.WriteLine("'append' property missed. 'append' set to 'False'.");
            }
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
            //message.Properties.Add("targetPath", @"C:/Users");
            //message.Properties.Add("targetFilename", @"Test.txt");
            //message.Properties.Add("containerName", @"data");
            //message.Properties.Add("filename", @"Test.txt");
            //message.Properties.Add("append", @"False");
            //*********************

            logReceivedMessage(message);

            #region Checking required properties
            if (!validateStringParams(message, "targetPath", "targetFilename", "containerName", "filename")) return MessageResponse.Abandoned;
            if (!validateStringParams(message, "append"))
            {
                message.Properties.Add("append", @"False");
                Console.WriteLine("'append' property missed. 'append' set to 'False'.");
            }
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
            //message.Properties.Add("sourcePath", @"C:/Users");
            //message.Properties.Add("sourceFilename", @"Test.txt");
            //*********************

            logReceivedMessage(message);

            #region Checking required properties
            if (!validateStringParams(message, "sourcePath", "sourceFilename")) return MessageResponse.Abandoned;
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
            //message.Properties.Add("sourcePath", @"C:/Users");
            //message.Properties.Add("sourceFilename", @"Test.txt");
            //message.Properties.Add("append", @"False");
            //*********************

            logReceivedMessage(message);

            #region Checking required properties
            if (!validateStringParams(message, "sourcePath", "sourceFilename")) return MessageResponse.Abandoned;

            if (!validateStringParams(message, "append"))
            {
                message.Properties.Add("append", @"False");
                Console.WriteLine("'append' property missed. 'append' set to 'False'.");
            }
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
            //message.Properties.Add("sourcePath", @"C:/Users");
            //message.Properties.Add("sourceFilename", @"Test.txt");
            //*********************

            logReceivedMessage(message);

            #region Checking required properties
            if (!validateStringParams(message, "sourcePath", "sourceFilename")) return MessageResponse.Abandoned;
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
            //message.Properties.Add("sourcePath", @"C:/Users");
            //*********************

            logReceivedMessage(message);

            #region Checking required properties
            if (!validateStringParams(message, "sourcePath")) return MessageResponse.Abandoned;
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

        static bool validateStringParams(Message message, params string[] propNames)
        {
            foreach (var propName in propNames)
            {
                if (!message.Properties.ContainsKey(propName))
                {
                    Console.WriteLine($"A required '{propName}' property is missed.");
                    return false;
                    //throw new ArgumentException("A required property is missed.", propName);
                }
                else
                {
                    string propValue = message.Properties[propName];
                    if(string.IsNullOrWhiteSpace(propValue))
                    {
                        Console.WriteLine($"A required '{propName}' property cannot be null or whitespace.");
                        return false;
                        //throw new ArgumentNullException(propName);
                    }
                }
            }

            return true;
        }
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