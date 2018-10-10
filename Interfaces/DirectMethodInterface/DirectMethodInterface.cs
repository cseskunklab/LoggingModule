using System;
using System.IO;
using System.Threading.Tasks;
using System.Text;
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

        private static string fixPath(string sourcePath)
        {
            return sourcePath.IndexOf("/") + 1 == sourcePath.Length ? sourcePath : sourcePath + "/";
        }
        
        private static string getContainerName(string containerName)
        {
            string container = null;

            if (!containerName.Contains('/'))
            {
                container = containerName;
            }
            else
            {
                string[] parts = containerName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                container = parts[0];
            }
            return container;
        }

        private static string getFilepathForContainer(string containerName, string targetFilename)
        {
            string path = null;

            if (!containerName.Contains('/'))
            {
                path = targetFilename;
            }
            else
            {
                string[] parts = containerName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                StringBuilder builder = new StringBuilder();
                if (parts.Length > 1)
                {
                    int index = 1;
                    while (index < parts.Length)
                    {
                        builder.Append(parts[index] + "/");
                        index++;
                    }
                    builder.Append(targetFilename);
                    path = builder.ToString();
                }
                else
                {
                    path = targetFilename;
                }
            }

            return path;
        }

        public async Task UploadFile(string sourcePath, string sourceFilename, string containerName, string targetFilename, string contentType, bool append=false)
        {

            byte[] fileContent = File.ReadAllBytes(Path.Join(fixPath(sourcePath), sourceFilename));
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(getContainerName(containerName));
            container.CreateIfNotExistsAsync().Wait();
            CloudAppendBlob blob = container.GetAppendBlobReference(getFilepathForContainer(containerName, targetFilename));

            if (!append)
            {
                await blob.CreateOrReplaceAsync();
            }
            else
            {   
                if (!blob.ExistsAsync().Result)
                {
                    throw new Exception($"Cannot append to nonexistent blob file {sourceFilename}"); 
                }
            }
            blob.Properties.ContentType = contentType;
            await blob.AppendTextAsync(fileContent.ToString());
        }

        public async Task UploadFile(string sourcePath, string sourceFilename, string sasUri, string contentType, bool append=false)
        {
            byte[] fileContent = File.ReadAllBytes(Path.Join(fixPath(sourcePath), sourceFilename));
            CloudAppendBlob blob = new CloudAppendBlob(new Uri(sasUri));

            if (!append)
            {
                await blob.CreateOrReplaceAsync();
            }
            else
            {
                if (!blob.ExistsAsync().Result)
                {
                    throw new Exception($"Cannot append to nonexistent blob file {sourceFilename}");
                }
            }
            blob.Properties.ContentType = contentType;
            await blob.AppendTextAsync(fileContent.ToString());
        }
        
        public async Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append=false)
        {

            string targetFullPath = Path.Join(fixPath(targetPath), targetFilename);
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(getContainerName(containerName));
            CloudAppendBlob blob = container.GetAppendBlobReference(getFilepathForContainer(containerName, filename));

            if (!append)
            {
                await blob.DownloadToFileAsync(targetFullPath, FileMode.Create);
            }
            else
            {
                if (!blob.ExistsAsync().Result)
                {
                    throw new Exception($"Cannot download nonexistent blob file {targetFilename}");
                }
                await blob.DownloadToFileAsync(targetFullPath, FileMode.Append);
            }
        }

        public async Task DownloadFile(string targetPath, string targetFilename, string sasUri, bool append=false)
        {
            string targetFullPath = Path.Join(fixPath(targetPath), targetFilename);
            CloudAppendBlob blob = new CloudAppendBlob(new Uri(sasUri));

            if (!append)
            {
                await blob.DownloadToFileAsync(targetFullPath, FileMode.Create);
            }
            else
            {
                if (!blob.ExistsAsync().Result)
                {
                    throw new Exception($"Cannot download nonexistent blob file {targetFilename}");
                }
                await blob.DownloadToFileAsync(targetFullPath, FileMode.Append);
            }           
        }

        public async Task TruncateFile(string sourcePath, string sourceFilename, int maxBytes)
        {
            string sourceFullPath = Path.Join(fixPath(sourcePath), sourceFilename);
            using (var stream = new FileStream(sourceFullPath, FileMode.Open))
            {
                byte[] buffer = new byte[stream.Length];
                if (maxBytes < stream.Length)
                {
                    stream.Read(buffer, 0, (int)stream.Length);
                    stream.Close();
                }
                using(var fileToTruncate = new FileStream(sourceFullPath, FileMode.Truncate))
                {
                    await fileToTruncate.WriteAsync(buffer, (int)buffer.Length - maxBytes, buffer.Length);
                }
            }
        }
    }
}
