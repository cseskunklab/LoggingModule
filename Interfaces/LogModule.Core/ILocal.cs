using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogModule.Core
{
    public interface ILocal : IRemote
    {
        Task<byte[]> GetFile(string sourcePath, string sourceFilename);
        Task WriteFile(string sourcePath, string sourceFilename, byte[] body, bool append);

        Task RemoveFile(string sourcePath, string sourceFilename);

        Task<string[]> ListFiles(string sourcePath);
    }
}
