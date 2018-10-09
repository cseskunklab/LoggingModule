using Objects;
using System;
using System.Threading.Tasks;

namespace Actions
{
    public class Actions
    {
    }

    public class Dummy
    {
        public Task uploadFile(UploadFileParams @params)
        {
            return Task.Delay(1000);
        }

        public Task downloadFile(DownloadFileParams @params)
        {
            return Task.Delay(1000);
        }

        public Task<byte[]> getFile(GetFileParams @params)
        {
            return Task.Run(() => new byte[1024]);
        }

        public Task writeFile(WriteFileParams @params)
        {
            return Task.Delay(1000);
        }

        public Task removeFile(RemoveFileParams @params)
        {
            return Task.Delay(1000);
        }

        public Task truncateFile(TruncateFileParams @params)
        {
            return Task.Delay(1000);
        }

        public Task listFile(ListFileParams @params)
        {
            return Task.Run(() => new string[3] {"file1.txt", "file2.txt", "file3.txt" });
        }

    }
}
