using System;
using Xunit;
using System.IO;

namespace LogModule.Core.Tests
{
    public class LocalFileTests
    {
        [Fact]
        public async void Should_GetException_When_FileIsNotFound()
        {
            // arrange
            var io = new LocalFileIO("", "");

            // act
            try
            {
                var result = await io.GetFile("/", "file.txt");
                Assert.Equal(0, result.Length);
            }
            catch(FileNotFoundException e) 
            {
                Assert.Equal(null, null);
            }
        }

        [Fact]
        public async void Should_GetByteArray_When_FileIsFound()
        {
            // arrange
            var io = new LocalFileIO("", "");

            // act
            var result = await io.GetFile("/", "test.txt");

            // assert
            Assert.Equal(2, result.Length);
        }
    }
}
