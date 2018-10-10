using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogModule.Core
{
    public class LocalFileIO : ILocal
    {
        public LocalFileIO()
        {

        }

        public async Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append = false)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> GetFile(string sourcePath, string sourceFilename)
        {
            throw new NotImplementedException();
        }

        public async Task<string[]> ListFiles(string sourcePath, string sourceFilename, int maxRows)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFile(string sourcePath, string sourceFilename)
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(string sourcePath, string sourceFilename, string containerName, string targetFilename, string contentType, bool append = false)
        {
            throw new NotImplementedException();
        }

        public Task UploadFile(string sourcePath, string sourceFilename, string sasUri, string contentType, bool append = false)
        {
            throw new NotImplementedException();
        }

        public Task WriteFile(string sourcePath, string sourceFilename, byte[] body, bool append)
        {
            throw new NotImplementedException();
        }
    }
}
