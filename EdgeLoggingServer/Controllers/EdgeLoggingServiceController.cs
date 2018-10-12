using System.Threading.Tasks;
using LogModule.Core;
using Microsoft.AspNetCore.Mvc;

namespace EdgeLoggingServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EdgeLoggingServiceController : WebApiService<ILocal>, ILocal
    {

        public EdgeLoggingServiceController(ILocal edgeLoggingService) : base(edgeLoggingService)
        {
        }

        public Task<string> GetTest()
        {
            return Task.FromResult<string>("hello");
        }

        [HttpPost("UploadFile")]
        public Task UploadFile(string sourcePath,
                        string sourceFilename,
                        string containerName,
                        string targetFilename,
                        string contentType,
                        bool append = false)
        {
            return InnerService.UploadFile(sourcePath, sourceFilename, containerName, targetFilename, contentType, append);
        }


        [HttpPost("sas/UploadFile")]
        public Task UploadFile(string sourcePath,
            string sourceFilename,
            string sasUri,
            string contentType,
            bool append = false)
        {
            return InnerService.UploadFile(sourcePath, sourceFilename, sasUri, contentType, append);
        }

        [HttpGet("DownloadFile")]
        public Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append = false)
        {
            return InnerService.DownloadFile(targetPath, targetFilename, containerName, filename, append);
        }

        [HttpGet("sas/DownloadFile")]
        public Task DownloadFile(string targetPath, string targetFilename, string sasUri, bool append = false)
        {
            return InnerService.DownloadFile(targetPath, targetFilename, sasUri, append);
        }

        [HttpGet("GetFile")]
        public Task<byte[]> GetFile(string sourcePath, string sourceFilename)
        {
            return InnerService.GetFile(sourcePath, sourceFilename);
        }

        [HttpGet("ListFiles")]
        public Task<string[]> ListFiles(string sourcePath)
        {
            return InnerService.ListFiles(sourcePath);
        }

        [HttpPut("WriteFile")]
        public Task WriteFile(string sourcePath, string sourceFilename, [FromBody] byte[] fileContent, bool append)
        {
            return  InnerService.WriteFile(sourcePath, sourceFilename, fileContent, append);  
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