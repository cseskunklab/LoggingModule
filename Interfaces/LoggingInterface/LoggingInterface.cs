using System;
using System.IO;
using System.Threading.Tasks;

namespace LoggingInterface
{
    public class LoggingInterface
    {
        public Task<byte[]> GetFile(string sourcePath, string sourceFilename)
        {
            return Task.Run(() => File.ReadAllBytes(sourcePath + @"\" + sourceFilename));
        }

        public Task WriteFile(string sourcePath, string sourceFilename, bool append)
        {

        }

        public Task RemoveFile(string sourcePath, string sourceFilename)
        {
            return Task.Run(() => File.Delete(sourcePath + @"\" + sourceFilename));
        }

        public Task<string[]> ListFiles(string sourcePath, string sourceFilename, int maxRows)
        {
            return Task.Run(() => Directory.GetFiles(sourcePath + @"\" + sourceFilename));
        }
    }
}
