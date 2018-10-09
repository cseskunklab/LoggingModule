using System;

namespace Objects
{
    public class UploadFileParams
    {
        public string sourcePath { get; set; }
        public string sourceFilename { get; set; }
        public string containerName { get; set; }
        public string sasUri { get; set; }
        public string targetFilename { get; set; }
        public string contentType { get; set; }
        public bool append { get; set; }
    }

    public class DownloadFileParams
    {
        public string targetPath { get; set; }
        public string targetFilename { get; set; }
        public string containerName { get; set; }
        public string sasUri { get; set; }
        public string filename { get; set; }
        public bool append { get; set; }
    }

    public class GetFileParams
    {
        public string sourcePath { get; set; }
        public string sourceFilename { get; set; }
    }

    public class WriteFileParams
    {
        public string sourcePath { get; set; }
        public string sourceFilename { get; set; }
        public bool append { get; set; }
        public byte[] data { get; set; }
    }

    public class RemoveFileParams
    {
        public string sourcePath { get; set; }
        public string sourceFilename { get; set; }
    }

    public class TruncateFileParams
    {
        public string sourcePath { get; set; }
        public string sourceFilename { get; set; }
        public int maxRows { get; set; }
    }

    public class ListFileParams
    {
        public string sourcePath { get; set; }
        public string sourceFilename { get; set; }
        public int maxRows { get; set; }
    }
}
