using System;
using System.Threading.Tasks;

namespace LogModule.Core
{
    public interface IRemote
    {
        Task UploadFile(string sourcePath, string sourceFilename, string containerName, string targetFilename, string contentType, bool append = false);
        Task UploadFile(string sourcePath, string sourceFilename, string sasUri, string contentType, bool append = false);

        Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append = false);

        
    }
}
