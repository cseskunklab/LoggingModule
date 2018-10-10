using System.Threading.Tasks;
using EdgeLoggingService;
using Microsoft.AspNetCore.Mvc;

namespace EdgeLoggingServer.Controllers
{
    [Route("[controller]/[action]")]
    [Controller]
    public class EdgeLoggingServiceController : WebApiService<IEdgeLoggingService>, IEdgeLoggingService
    {

        public EdgeLoggingServiceController(IEdgeLoggingService edgeLoggingService) : base(edgeLoggingService)
        {
        }

        public Task<string> GetTest()
        {
            return Task.FromResult<string>("hello");
        }

        [HttpPost]
        public Task UploadFile(string sourcePath,
                        string sourceFilename,
                        string containerName,
                        string targetFilename,
                        string contentType,
                        bool append = false)
        {
            return InnerService.UploadFile(sourcePath, sourceFilename, containerName, targetFilename, contentType, append);
        }


        [HttpPost]
        public Task UploadFile2(string sourcePath,
            string sourceFilename,
            string sasUri,
            string contentType,
            bool append = false)
        {
            return InnerService.UploadFile2(sourcePath, sourceFilename, sasUri, contentType, append);
        }

        [HttpGet("DownloadFile")]
        public Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append = false)
        {
            return InnerService.DownloadFile(targetPath, targetFilename, containerName, filename, append);
        }

        [HttpGet("DownloadFile2")]
        public Task DownloadFile2(string targetPath, string targetFilename, string sasUri, bool append = false)
        {
            return InnerService.DownloadFile2(targetPath, targetFilename, sasUri, append);
        }

        [HttpGet("GetFile")]
        public Task<byte[]> GetFile(string sourcePath, string sourceFilename)
        {
            return InnerService.GetFile(sourcePath, sourceFilename);
        }

        [HttpGet("ListFiles")]
        public Task<string[]> ListFiles(string sourcePath, string sourceFilename, int maxRows)
        {
            return InnerService.ListFiles(sourcePath, sourceFilename, maxRows);
        }

        [HttpPut("WriteFile")]
        public Task WriteFile(string sourcePath, string sourceFilename, [FromBody] byte[] fileContent)
        {
            return InnerService.WriteFile(sourcePath, sourceFilename, fileContent); ;
        }

        [HttpDelete("RemoveFile")]
        public Task RemoveFile(string sourcePath, string sourceFilename)
        {
            return InnerService.RemoveFile(sourcePath, sourceFilename);
        }

        [HttpPut("TruncateFile")]
        public Task TruncateFile(string sourcePath, string sourceFilename, int maxRows)
        {
            return InnerService.TruncateFile(sourceFilename, sourceFilename, maxRows);
        }

    }
}
