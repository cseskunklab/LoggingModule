using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DirectMethodInterface
{
    public class DirectMethodInterface
    {
        private readonly string accountName;
        private readonly string accountKey;
        private readonly string storageConnectionString; 

        public DirectMethodInterface(string accountName, string accountKey)
        {
            this.accountName = accountName;
            this.accountKey = accountKey;
            this.storageConnectionString = "DefaultEndpointsProtocol=https;" + "AccountName=" + accountName + ";AccountKey=" + accountKey + ";EndpointSuffix=core.windows.net";
        }

        public Task UploadFile(string sourcePath, string sourceFilename, string containerName, string targetFilename, string contentType, bool append=false)
        {
            string fileContent = File.ReadAllText(sourcePath + "/" + sourceFilename);
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().Wait();

            CloudAppendBlob blob = container.GetAppendBlobReference(targetFilename);
            if (!append)
            {
                blob.CreateOrReplaceAsync();
            }
            else
            {   
                if (!blob.ExistsAsync().Result)
                {
                    throw new Exception("Cannot append to nonexistent blob file"); 
                }
            }
            blob.Properties.ContentType = contentType;
            return blob.AppendTextAsync(fileContent);
        }

        public Task UploadFile(string sourcePath, string sourceFilename, string sasUri, string contentType, bool append=false)
        {
            string fileContent = File.ReadAllText(sourcePath + @"\" + sourceFilename);

            CloudAppendBlob blob = new CloudAppendBlob(new Uri(sasUri));

            if (!append)
            {
                blob.CreateOrReplaceAsync();
            }
            else
            {
                if (!blob.ExistsAsync().Result)
                {
                    throw new Exception("Cannot append to nonexistent blob file");
                }
            }
            blob.Properties.ContentType = contentType;
            return blob.AppendTextAsync(fileContent);
        }
        
        public Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append=false)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(containerName);

            CloudAppendBlob blob = container.GetAppendBlobReference(filename);

            if (!append)
            {
                return blob.DownloadToFileAsync(targetPath + @"\" + targetFilename, FileMode.Create);
            }
            else
            {
                if (!blob.ExistsAsync().Result)
                {
                    throw new Exception("Cannot append to nonexistent blob file");
                }
                return blob.DownloadToFileAsync(targetPath + @"\" + targetFilename, FileMode.Append);
            }
        }

        public Task DownloadFile(string targetPath, string targetFilename, string sasUri, bool append=false)
        {
            CloudAppendBlob blob = new CloudAppendBlob(new Uri(sasUri));

            if (!append)
            {
                return blob.DownloadToFileAsync(targetPath + @"\" + targetFilename, FileMode.Create);
            }
            else
            {
                if (!blob.ExistsAsync().Result)
                {
                    throw new Exception("Cannot append to nonexistent blob file");
                }
                return blob.DownloadToFileAsync(targetPath + @"\" + targetFilename, FileMode.Append);
            }           
        }

        public Task TruncateFile(string sourcePath, string sourceFilename, int maxBytes)
        {
            FileStream fs = new FileStream(sourcePath + @"\" + sourceFilename, FileMode.Open);
            if(maxBytes < fs.Length)
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Close();
                FileStream fs2 = new FileStream(sourcePath + "/" + sourceFilename, FileMode.Truncate);
                fs2.Write(buffer, (int)buffer.Length - maxBytes, buffer.Length);
            }
        }
    }
}
