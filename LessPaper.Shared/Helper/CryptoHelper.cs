﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace LessPaper.Shared.Helper
{
    public static class CryptoHelper
    {
        private static readonly RNGCryptoServiceProvider SecureRandomProvider = new RNGCryptoServiceProvider();

        /// <summary>
        /// Generates a Sha256 hash from the concatenation of the given values
        /// </summary>
        /// <param name="value">Main value</param>
        /// <param name="salt">Random salt</param>
        /// <returns>Hash as Base64 format</returns>
        public static string Sha256FromString(string value, string salt)
        {
            using var sha = SHA256.Create();
            var saltedPassword = $"{salt}{value}";
            var saltedPasswordAsBytes = Encoding.UTF8.GetBytes(saltedPassword);
            return Convert.ToBase64String(sha.ComputeHash(saltedPasswordAsBytes));
        }

        /// <summary>
        /// Generates a secure salt
        /// </summary>
        /// <param name="byteCount">Number of bytes</param>
        /// <returns>Salt</returns>
        public static string GetSalt(uint byteCount = 10)
        {
            var byteArray = new byte[byteCount];
            SecureRandomProvider.GetBytes(byteArray);
            return Convert.ToBase64String(byteArray);
        }


        public class RsaKeyPair
        {
            public RsaKeyPair(string publicKey, string privateKey)
            {
                PublicKey = publicKey;
                PrivateKey = privateKey;
            }

            public string PublicKey { get; }

            public string PrivateKey { get;  }
        }

        public static RsaKeyPair GenerateRsaKeyPair(int keySize = 2048)
        {
            using var rsa = new RSACryptoServiceProvider(keySize);

            var publicKey = rsa.ExportParameters(false);
            string serializedPublicKey;
            using (var sw = new StringWriter())
            {
                var xs = new XmlSerializer(typeof(RSAParameters));
                xs.Serialize(sw, publicKey);
                serializedPublicKey = sw.ToString();
            }

            var privateKey = rsa.ExportParameters(true);
            string serializedPrivateKey;
            using (var sw = new StringWriter())
            {
                var xs = new XmlSerializer(typeof(RSAParameters));
                xs.Serialize(sw, privateKey);
                serializedPrivateKey = sw.ToString();
            }

            return new RsaKeyPair(serializedPublicKey, serializedPrivateKey);
        }

        public static string RsaEncrypt(string serializedPublicKey, string data, int keySize = 2048)
        {
            using var sr = new StringReader(serializedPublicKey);
            var xs = new XmlSerializer(typeof(RSAParameters));
            var publicKey = (RSAParameters)xs.Deserialize(sr);

            using var rsa = new RSACryptoServiceProvider(keySize);
            rsa.ImportParameters(publicKey);

            var dataAsBytes = Encoding.UTF8.GetBytes(data);
            var ciphertext = rsa.Encrypt(dataAsBytes, true);

            return Convert.ToBase64String(ciphertext);
        }


        public static string AesEncrypt(string key, string iv, string plainText)
        {
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.GenerateIV();
                rijAlg.GenerateKey();

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                
                using MemoryStream msEncrypt = new MemoryStream();
                using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using StreamWriter swEncrypt = new StreamWriter(csEncrypt);
               
                swEncrypt.Write(plainText);

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }


    }
}
