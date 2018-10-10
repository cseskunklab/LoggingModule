using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogModule.Core
{
    public class RemoteFileIO : IRemote
    {
        public RemoteFileIO()
        {

        }

        public async Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append = false)
        {
            throw new NotImplementedException();
        }

        public async Task UploadFile(string sourcePath, string sourceFilename, string containerName, string targetFilename, string contentType, bool append = false)
        {
            throw new NotImplementedException();
        }

        public async Task UploadFile(string sourcePath, string sourceFilename, string sasUri, string contentType, bool append = false)
        {
            throw new NotImplementedException();
        }
    }
}
