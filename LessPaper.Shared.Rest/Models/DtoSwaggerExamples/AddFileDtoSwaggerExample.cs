using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Rest.Models.RequestDtos;
using Swashbuckle.AspNetCore.Filters;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    public class AddFileDtoSwaggerExample : IExamplesProvider<AddFileDto>
    {
        /// <inheritdoc />
        public AddFileDto GetExamples()
        {
            return new AddFileDto()
            {
                DocumentLanguage = DocumentLanguage.German,
                EncryptedKey = new Dictionary<string, string>
                {
                    {
                        "0146613259928c4bd8a58dfd0fca344e47",
                        CryptoHelper.RsaEncrypt(
                            "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Exponent>AQAB</Exponent>\r\n  <Modulus>p1uEX6YqAWKKQVJaAv/m3o2w5xsa/3xdVwPujlRilwXjpjOYhlxiXctd382RNzzGjozNAhCJH3DKPrga81HeKPGuQXIDfHrZ7kVANt/bD+YzAiwuTTgK0MuNmAOFz3AVuenNfJ7RyzrbKR6eZSvn1wbi2HHrdHSZ9nthTJbROQliYjLedwX4ToU++mGyWTDTkP9MA4zAx/s+qeEnNMAn9Q7Mhy0m6TD2sdrBtptX4XIk7Hl62BlD10edmSv/xCbAVgzutkFzsKH+rwRJLR5KAwYRKGcOHnu6NhzFiHeeqHHe1vJ8fD3uytCLtwNwZFHvvS8rXhlq/Sk7hVFQeM76uQ==</Modulus>\r\n</RSAParameters>",
                            "00000000000"
                        )
                    }
                },
                FileExtension = ExtensionType.Pdf,
                FileName = "TestDocument",
                FileSize = 1000
            };
        }
    }
}
