using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LoggingInterface
{
    public class LoggingInterface
    {
        public async Task<byte[]> GetFileAsync(string sourcePath, string sourceFilename)
        {
            return await File.ReadAllBytesAsync(Path.Join(sourcePath, sourceFilename));
        }

        public async Task WriteFileAsync(string sourcePath, string sourceFilename, byte[] body, bool append)
        {
            string sourceFullPath = Path.Join(sourcePath, sourceFilename);
            if (append)
            {
                if(!File.Exists(sourceFullPath)) 
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

        public async Task RemoveFile(string sourcePath, string sourceFilename)
        {
            await Task.Run(() => File.Delete(Path.Join(sourcePath, sourceFilename)));
        }

        public async Task<string[]> ListFilesAsync(string sourcePath, string sourceFilename, int maxRows)
        {
            return await Task.Run(() => Directory.GetFiles(Path.Join(sourcePath, sourceFilename)));
        }
    }
}
