using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.APIGateway.Helper;
using Xunit;

namespace LessPaper.APIGateway.UnitTest.Helper
{
    public class ValidationHelperTest
    {
        [Fact]
        public void ValidateEmailAddress()
        {
            Assert.False(ValidationHelper.IsValidEmailAddress("a"));
            Assert.False(ValidationHelper.IsValidEmailAddress("a.de"));
            Assert.False(ValidationHelper.IsValidEmailAddress("a@.de"));
            Assert.False(ValidationHelper.IsValidEmailAddress("@b.de"));
            Assert.True(ValidationHelper.IsValidEmailAddress("a@b.de"));
        }
        
    }
}
