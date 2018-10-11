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
            //var container = GetContainerReference("tests");
            //container.DeleteIfExistsAsync().Wait();
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

        #endregion
    }
}
