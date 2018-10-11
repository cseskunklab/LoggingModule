using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LogModule.Core.Tests
{
    [TestClass]
    public class RemoteFileIOTests
    {
        [TestInitialize]
        public void Initialize()
        {
        }

        [TestCleanup]
        public void Teardown()
        {
            var container = GetContainerReference("tests");
            container.DeleteIfExistsAsync().Wait();
        }

        [TestMethod]
        public void UploadFile()
        {
            var remoteio = new RemoteFileIO("mshatest", "68J2t5EYJ6IF4vE6jiPfJwVOfyUnt1jdPCMPrvFDtOPBwZQtbvcAd1T/5PBFNzQyPG3m7v7hQYBRPc/YRPiqiA==");
            var localio = new LocalFileIO("foo", "bar");

            localio.WriteFile(".", "test1.txt", GenerateTestBytes(), false).Wait();

            remoteio.UploadFile(".", "test1.txt", "tests", "test1", "text/plain").Wait();

            var container = GetContainerReference("tests");

            var blob = container.GetBlobReference("test1");
            Assert.IsTrue(blob.ExistsAsync().Result);
        }

        #region Helpers

        private static byte[] GenerateTestBytes()
        {
            var bytes = Encoding.UTF8.GetBytes("hello tests!");

            return bytes;
        }

        private static void CleanupAllTestFiles()
        {
            var testFiles = Directory.GetFiles(".", "Test*.txt");
            foreach (var testFile in testFiles)
            {
                File.Delete(testFile);
                Debug.WriteLine("Deleted " + testFile);
            }
        }

        private CloudBlobContainer GetContainerReference(string containerName)
        {
            const string connectionString = "DefaultEndpointsProtocol=https;AccountName=mshatest;AccountKey=68J2t5EYJ6IF4vE6jiPfJwVOfyUnt1jdPCMPrvFDtOPBwZQtbvcAd1T/5PBFNzQyPG3m7v7hQYBRPc/YRPiqiA==;EndpointSuffix=core.windows.net";

            var container = CloudStorageAccount.Parse(connectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(containerName);

            return container;
        }

        #endregion
    }
}
