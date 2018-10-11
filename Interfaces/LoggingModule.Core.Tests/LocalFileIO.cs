using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace LogModule.Core.Tests
{
    [TestClass]
    public class LocalFileIOTests
    {
        [TestInitialize]
        public void Initialize()
        {
            CleanupAllTestFiles();
        }

        [TestCleanup]
        public void Teardown()
        {
            CleanupAllTestFiles();
        }

        [TestMethod]
        public void WriteAndReadFile_RoundtripData()
        {
            var io = new LocalFileIO("foo", "bar");
            io.WriteFile(".", "test1.txt", GenerateTestBytes(), false).Wait();

            var outFileBytes = io.GetFile(".", "test1.txt").Result;

            Assert.AreEqual("hello tests!", Encoding.UTF8.GetString(outFileBytes));
        }

        [TestMethod]
        public void WriteAndDeleteFile_FileGone()
        {
            var io = new LocalFileIO("foo", "bar");
            io.WriteFile(".", "test2.txt", GenerateTestBytes(), false).Wait();
            io.RemoveFile(".", "test2.txt").Wait();

            Assert.IsFalse(File.Exists(".\\test2.txt"));
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
            foreach(var testFile in testFiles)
            {
                File.Delete(testFile);
                Debug.WriteLine("Deleted " + testFile);
            }
        }

        #endregion
    }
}
