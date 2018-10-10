using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LogModule.Core
{
    public class LocalFileIO : RemoteFileIO, ILocal
    {
        //private readonly string accountName;
        //private readonly string accountKey;
        //private readonly string storageConnectionString;

    public LocalFileIO(string accountName, string accountKey)
    : base(accountName, accountKey)
        {

        }
   
           

        //public async Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append = false)
        //{
        //    //Task task = Task.Factory.StartNew(() =>
        //    //{
        //    //    string targetFullPath = Path.Join(fixPath(targetPath), targetFilename);
        //    //    CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
        //    //    CloudBlobClient serviceClient = account.CreateCloudBlobClient();

        //    //    var container = serviceClient.GetContainerReference(getContainerName(containerName));
        //    //    CloudAppendBlob blob = container.GetAppendBlobReference(getFilepathForContainer(containerName, filename));

        //    //    if (!append)
        //    //    {
        //    //        blob.DownloadToFileAsync(targetFullPath, FileMode.Create);
        //    //    }
        //    //    else
        //    //    {
        //    //        if (!blob.ExistsAsync().Result)
        //    //        {
        //    //            throw new Exception($"Cannot download nonexistent blob file {targetFilename}");
        //    //        }
        //    //        blob.DownloadToFileAsync(targetFullPath, FileMode.Append);
        //    //    }
        //    //});

        //    //await Task.WhenAll(task);
        //}

        public async Task<byte[]> GetFile(string sourcePath, string sourceFilename)
        {            
            Task<byte[]> task = Task.Factory.StartNew(() =>
            {
                string path = fixPath(sourcePath) + sourceFilename;
                return File.ReadAllBytes(path);
            });

            await Task.WhenAll(task);
            return task.Result;
        }


        public async Task<string[]> ListFiles(string sourcePath, string sourceFilename)
        {
            Task<string[]> task = Task.Factory.StartNew(() =>
            {
                string path = fixPath(sourcePath) + sourceFilename;
                return Directory.GetFiles(path);
            });

            await Task.WhenAll(task);
            return task.Result;
        }

        public async Task RemoveFile(string sourcePath, string sourceFilename)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                string path = fixPath(sourcePath) + sourceFilename;
                File.Delete(path);
            });

            await Task.WhenAll(task);
        }

        //public async Task UploadFile(string sourcePath, string sourceFilename, string containerName, string targetFilename, string contentType, bool append = false)
        //{
        //    Task task = Task.Factory.StartNew(() =>
        //    {
        //        byte[] fileContent = File.ReadAllBytes(Path.Join(fixPath(sourcePath), sourceFilename));
        //        CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
        //        CloudBlobClient serviceClient = account.CreateCloudBlobClient();

        //        var container = serviceClient.GetContainerReference(getContainerName(containerName));
        //        container.CreateIfNotExistsAsync().Wait();
        //        CloudAppendBlob blob = container.GetAppendBlobReference(getFilepathForContainer(containerName, targetFilename));

        //        if (!append)
        //        {
        //            blob.CreateOrReplaceAsync();
        //        }
        //        else
        //        {
        //            if (!blob.ExistsAsync().Result)
        //            {
        //                throw new Exception($"Cannot append to nonexistent blob file {sourceFilename}");
        //            }
        //        }
        //        blob.Properties.ContentType = contentType;
        //        blob.AppendTextAsync(fileContent.ToString());
        //    });

        //    await Task.WhenAll(task);
        //}

        //public async Task UploadFile(string sourcePath, string sourceFilename, string sasUri, string contentType, bool append = false)
        //{
        //    Task task = Task.Factory.StartNew(() =>
        //    {
        //        byte[] fileContent = File.ReadAllBytes(Path.Join(fixPath(sourcePath), sourceFilename));
        //        CloudAppendBlob blob = new CloudAppendBlob(new Uri(sasUri));

        //        if (!append)
        //        {
        //            blob.CreateOrReplaceAsync();
        //        }
        //        else
        //        {
        //            if (!blob.ExistsAsync().Result)
        //            {
        //                throw new Exception($"Cannot append to nonexistent blob file {sourceFilename}");
        //            }
        //        }
        //        blob.Properties.ContentType = contentType;
        //        blob.AppendTextAsync(fileContent.ToString());
        //    });

        //    await Task.WhenAll(task);
        //}

        public async Task WriteFile(string sourcePath, string sourceFilename, byte[] body, bool append)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                string sourceFullPath = Path.Join(sourcePath, sourceFilename);
                if (append)
                {
                    if (!File.Exists(sourceFullPath))
                    {
                        throw new Exception($"Cannot append to nonexistent file {sourceFilename}");
                    }

                    byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                    using (var stream = new FileStream(sourceFullPath, FileMode.Append))
                    {
                        stream.Write(newline, 0, newline.Length);
                        stream.Write(body, 0, body.Length);
                    }
                }
                File.WriteAllBytesAsync(sourceFullPath, body);
            });

            await Task.WhenAll(task);
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
    }
}
