using System;
using LessPaper.Shared.MinIO.Models;
using Xunit;

namespace LessPaper.Shared.MinIO.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var e = new MinioSettings("127.0.0.1:9000", "minioadmin", "minioadmin");
            //TODO Implement unit tests
        }
    }
}
