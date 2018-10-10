using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EdgeLoggingService
{
    public class EdgeLoggingService : IEdgeLoggingService
    {
        public Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append = false)
        {
            return Task.CompletedTask;
        }

        public Task DownloadFile2(string targetPath, string targetFilename, string sasUri, bool append = false)
        {
            return Task.CompletedTask;
        }

        public Task<byte[]> GetFile(string sourcePath, string sourceFilename)
        {
            return Task.FromResult<byte[]>(new byte[] { });
        }

        public Task<string[]> ListFiles(string sourcePath, string sourceFilename, int maxRows)
        {
            return Task.FromResult<string[]>(new string[] { });
        }

        public Task RemoveFile(string sourcePath, string sourceFilename)
        {
            return Task.CompletedTask;
        }

        public Task TruncateFile(string sourcePath, string sourceFilename, int maxRows)
        {
            return Task.CompletedTask;
        }

        public Task UploadFile(string sourcePath, string sourceFilename, string containerName, string targetFilename, string contentType, bool append = false)
        {
            return Task.CompletedTask;
        }

        public Task UploadFile2(string sourcePath, string sourceFilename, string sasUri, string contentType, bool append = false)
        {
            return Task.CompletedTask;
        }

        public Task WriteFile(string sourcePath, string sourceFilename, byte[] fileContent)
        {
            return Task.CompletedTask;
        }
    }
}
