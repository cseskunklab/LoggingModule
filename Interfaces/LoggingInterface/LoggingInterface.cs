using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LoggingInterface
{
    public class LoggingInterface
    {
        public Task<byte[]> GetFile(string sourcePath, string sourceFilename)
        {
            return File.ReadAllBytesAsync(Path.Join(sourcePath, sourceFilename));
        }

        public Task WriteFile(string sourcePath, string sourceFilename, byte[] body, bool append)
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
            return File.WriteAllBytesAsync(sourceFullPath, body);
        }

        public void RemoveFile(string sourcePath, string sourceFilename)
        {
            File.Delete(Path.Join(sourcePath, sourceFilename));
        }

        public string[] ListFiles(string sourcePath, string sourceFilename, int maxRows)
        {
            return Directory.GetFiles(Path.Join(sourcePath, sourceFilename));
        }
    }
}
