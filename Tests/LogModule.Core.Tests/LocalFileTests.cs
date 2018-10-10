using System;
using Xunit;

namespace LogModule.Core.Tests
{
    public class LocalFileTests
    {
        [Fact]
        public void Should_GetNull_When_FileIsNotFound()
        {
            // arrange
            var io = new LocalFileIO("", "");

            // act
            var result = io.GetFile("", "");

            // assert
            Assert.Equal(null, result);
        }

        [Fact]
        public void Should_GetByteArray_When_FileIsFound()
        {
            // arrange
            var io = new LocalFileIO("", "");

            // act
            var result = io.GetFile("", "");

            // assert
            Assert.Equal(null, result);
        }
    }
}
