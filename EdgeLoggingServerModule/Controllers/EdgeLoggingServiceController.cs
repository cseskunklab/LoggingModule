using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FieldGatewayMicroservice.Connections;
using FieldGatewayMicroservice.Models;
using Microsoft.AspNetCore.Mvc;

namespace FieldGatewayMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EdgeLoggingServiceController : WebApiService<IEdgeLoggingService>, IEdgeLoggingService
    {
        private string requestUrl; // = "http://echomodule:8889/api/rtuinput";



        public EdgeLoggingServiceController(IEdgeLoggingService edgeLoggingService) : base(edgeLoggingService)
        {
           
            //string url = System.Environment.GetEnvironmentVariable("RTU_InputUrl");
            //if(!string.IsNullOrEmpty(url))
            //{
            //    requestUrl = url;
            //}
        }

        //[HttpGet]
        //public HttpResponseMessage Get()
        //{
        //    HttpResponseMessage response = new HttpResponseMessage();
        //    response.Content = new StringContent("{ pie: \"hello\" }");
        //    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        //    response.StatusCode = HttpStatusCode.OK;
        //    return response;
        //}

        //[HttpPost]
        //public async Task Post([FromBody] byte[] value)
        //{
        //    try
        //    {
        //        Console.WriteLine("{0} - Start echo forward to Piraeus", DateTime.Now.ToString("hh:MM:ss.ffff"));
        //        await EdgeClient.Client.PublishAsync(EdgeClient.Config.Resources.RtuOutputResource, value);
        //        Console.WriteLine("{0} - End echo forward to Piraeus", DateTime.Now.ToString("hh:MM:ss.ffff"));
        //        return new HttpResponseMessage(HttpStatusCode.OK);
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine("Fault publishing MQTT message '{0}'", ex.Message);
        //        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //    }


        [HttpPost]
        Task UploadFile(string sourcePath,
                        string sourceFilename,
                        string containerName,
                        string targetFilename,
                        string contentType,
                        bool append = false)
        {
            return InnerService.UploadFile(sourcePath, sourceFilename, containerName, targetFilename, contentType, append);
        }

    
       [HttpPost]
        public Task UploadFile(string sourcePath, string sourceFilename, string sasUri, string contentType, bool append = false)
        {
            return InnerService.UploadFile(sourcePath, sourceFilename, sasUri, contentType, append);
        }

        [HttpGet]
        public Task DownloadFile(string targetPath, string targetFilename, string containerName, string filename, bool append = false)
        {
            return InnerService.DownloadFile(targetPath, targetFilename, containerName, filename, append);
        }

        [HttpGet]
        public Task DownloadFile(string targetPath, string targetFilename, string sasUri, bool append = false)
        {
            return InnerService.DownloadFile(targetPath, targetFilename, sasUri, append);
        }

        [HttpGet]
        public Task<byte[]> GetFile(string sourcePath, string sourceFilename)
        {
            return InnerService.GetFile(sourcePath, sourceFilename);
        }

        [HttpGet]
        public Task<string[]> ListFiles(string sourcePath, string sourceFilename, int maxRows)
        {
            return InnerService.ListFiles(sourcePath, sourceFilename, maxRows);
        }

        [HttpPut]
        public Task WriteFile(string sourcePath, string sourceFilename, byte[] fileContent)
        {
            return InnerService.WriteFile(sourcePath, sourceFilename, fileContent); ;
        }

        [HttpDelete]
        public Task RemoveFile(string sourcePath, string sourceFilename)
        {
            return InnerService.RemoveFile(sourcePath, sourceFilename);
        }

        [HttpPut]
        public Task TruncateFile(string sourcePath, string sourceFilename, int maxRows)
        {
            return InnerService.TruncateFile(sourceFilename, sourceFilename, maxRows);
        }


        //private async Task<HttpStatusCode> ForwardAsync(byte[] payload)
        //{
        //    BinaryOutputFormatter2 formatter = new BinaryOutputFormatter2();
        //    HttpClient client = new HttpClient();
        //    HttpContent content = new ByteArrayContent(payload);
        //    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        //    content.Headers.ContentLength = payload.Length;
        //    HttpResponseMessage response = await client.PostAsync(requestUrl, content);
        //    return response.StatusCode;
        //}


    }
}
