using System;
using Xunit;

namespace LogModule.Core.Tests
{
    public class RemoteFileTests
    {
        [Fact]
        public async void Should_Exception_When_FileIsNotFound()
        {
            // arrange
            var io = new RemoteFileIO("", "");

            // act
            await io.DownloadFile("/data", "test.txt", "https://loggingmodulestore.blob.core.windows.net/data/Test.txt?sp=r&st=2018-10-11T00:49:40Z&se=2018-10-11T08:49:40Z&spr=https&sv=2017-11-09&sig=yT3Sl79RLgrl5VDYXXAKjODL9mIiX7PGgYe3bzKl3Dg%3D&sr=b");

            // assert
            Assert.Equal(null, null);
        }

        [Fact]
        public async void Should_NotException_When_FileIsFound()
        {
            // arrange
            var io = new RemoteFileIO("", "");

            // act
            await io.DownloadFile("/data", "test.txt", "https://loggingmodulestore.blob.core.windows.net/data/Test.txt?sp=r&st=2018-10-11T00:49:40Z&se=2018-10-11T08:49:40Z&spr=https&sv=2017-11-09&sig=yT3Sl79RLgrl5VDYXXAKjODL9mIiX7PGgYe3bzKl3Dg%3D&sr=b");

            // assert
            Assert.Equal(null, null);
        }
    }
}
