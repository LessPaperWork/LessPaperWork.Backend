using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using LessPaper.Shared.Helper;
using Xunit;

namespace LessPaper.Shared.UnitTest
{
    public class CryptoHelperTest
    {

        [Fact]
        public void GetSalt()
        {
            // Check for duplicate salt
            var salts = new HashSet<string>();
            for (var i = 0; i < 50; i++)
            {
                var salt = CryptoHelper.GetSalt(10);
                Assert.NotNull(salt);
                Assert.False(string.IsNullOrWhiteSpace(salt));
                Assert.DoesNotContain(salt, salts);
                salts.Add(salt);
            }

            Assert.NotEmpty(salts);
        }
        
        [Fact]
        public void Sha256FromString()
        {
            // Deterministic
            var hash1 = CryptoHelper.Sha256FromString("a", "b");
            var hash2 = CryptoHelper.Sha256FromString("a", "b");
            Assert.Equal(hash1, hash2);
            
            // Salt is effective
            var hash3 = CryptoHelper.Sha256FromString("a", "a");
            var hash4 = CryptoHelper.Sha256FromString("a", "b");
            Assert.NotEqual(hash3, hash4);

            // Value is effective
            var hash5 = CryptoHelper.Sha256FromString("a", "a");
            var hash6 = CryptoHelper.Sha256FromString("b", "a");
            Assert.NotEqual(hash5, hash6);
        }

        [Fact]
        public void Bench()
        {
            var kp = CryptoHelper.GenerateRsaKeyPair();



            var start = DateTime.UtcNow;
            for (int i = 0; i < 100000; i++)
            {
                var e = CryptoHelper.AesEncrypt("", "", CryptoHelper.GetSalt(16));
            }
            var duration2 = DateTime.UtcNow - start;


            start = DateTime.UtcNow;
            for (int i = 0; i < 10000; i++)
            {
                var e = CryptoHelper.RsaEncrypt(kp.PublicKey, CryptoHelper.GetSalt(16));
            }

            var duration1 = DateTime.UtcNow - start;

            Console.WriteLine("d");
        }
    }
}
