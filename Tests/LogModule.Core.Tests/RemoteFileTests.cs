using System;
using Xunit;

namespace LogModule.Core.Tests
{
    public class RemoteFileTests
    {
        [Fact]
        public void Should_Exception_When_FileIsNotFound()
        {
            // arrange
            var io = new RemoteFileIO("", "");

            // act
            var result = io.DownloadFile("", "", "");

            // assert
            Assert.Equal(null, result);
        }

        [Fact]
        public void Should_NotException_When_FileIsFound()
        {
            // arrange
            var io = new RemoteFileIO("", "");

            // act
            var result = io.DownloadFile("", "", "");

            // assert
            Assert.Equal(null, result);
        }
    }
}
