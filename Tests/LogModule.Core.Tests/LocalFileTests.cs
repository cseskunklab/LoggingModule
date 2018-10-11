using System;
using Xunit;

namespace LogModule.Core.Tests
{
    public class LocalFileTests
    {
        [Fact]
        public async void Should_GetNull_When_FileIsNotFound()
        {
            // arrange
            var io = new LocalFileIO("", "");

            // act
            var result = await io.GetFile("/", "file.txt");

            // assert
            Assert.Equal(0, result.Length);
        }

        [Fact]
        public async void Should_GetByteArray_When_FileIsFound()
        {
            // arrange
            var io = new LocalFileIO("", "");

            // act
            var result = await io.GetFile("/", "file.txt");

            // assert
            Assert.Equal(0, result.Length);
        }
    }
}
