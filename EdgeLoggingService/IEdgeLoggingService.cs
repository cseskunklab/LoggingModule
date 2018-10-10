using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdgeLoggingService
{
    public interface IEdgeLoggingService
    {
        Task UploadFile(string sourcePath,
                string sourceFilename,
                string containerName,
                string targetFilename,
                string contentType,
                bool append = false);

        Task UploadFile2(string sourcePath,
                string sourceFilename,
                string sasUri,
                string contentType,
                bool append = false);

        Task DownloadFile(string targetPath,
                string targetFilename,
                string containerName,
                string filename,
                bool append = false);

        Task DownloadFile2(string targetPath,
                string targetFilename,
                string sasUri,
                bool append = false);

        Task<byte[]> GetFile(string sourcePath,
                string sourceFilename);

        Task<string[]> ListFiles(string sourcePath,
                string sourceFilename,
                int maxRows);

        Task WriteFile(string sourcePath,
                string sourceFilename,
                byte[] fileContent);


        Task RemoveFile(string sourcePath,
                string sourceFilename);

        Task TruncateFile(string sourcePath,
                string sourceFilename,
                int maxRows);
    }
}
