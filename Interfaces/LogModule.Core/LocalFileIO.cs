using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LogModule.Core
{
    public class LocalFileIO : RemoteFileIO, ILocal
    {
        public LocalFileIO(string connectionString) : base(connectionString)
        {
        }

        public LocalFileIO(string accountName, string accountKey) : base(accountName, accountKey)
        {
        }

        public async Task<byte[]> GetFile(string sourcePath, string sourceFilename)
        {
            string path = fixPath(sourcePath) + sourceFilename;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, (int)stream.Length);
                return bytes;
            }
        }

        public Task<string[]> ListFiles(string sourcePath)
        {
            string path = fixPath(sourcePath);
            return Task.FromResult(Directory.GetFiles(path));
        }

        public async Task RemoveFile(string sourcePath, string sourceFilename)
        {
            string path = fixPath(sourcePath) + sourceFilename;
            await Task.Run(() => {
                File.Delete(path);
            });
        }

        public async Task WriteFile(string sourcePath, string sourceFilename, byte[] body, bool append)
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
            await File.WriteAllBytesAsync(sourceFullPath, body);
        }

        private static string fixPath(string sourcePath)
        {
            return sourcePath[sourcePath.Length - 1] == '/' ? sourcePath : sourcePath + "/";
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
