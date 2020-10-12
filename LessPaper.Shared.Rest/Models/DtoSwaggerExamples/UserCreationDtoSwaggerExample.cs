using System;
using System.Collections.Generic;
using System.Text;
using LessPaper.Shared.Enums;
using LessPaper.Shared.Helper;
using LessPaper.Shared.Rest.Models.RequestDtos;
using Swashbuckle.AspNetCore.Filters;

namespace LessPaper.Shared.Rest.Models.DtoSwaggerExamples
{
    public class UserCreationDtoSwaggerExample : IExamplesProvider<UserCreationDto>
    {
        private string password = "0000000000000000";
        private string salt = "0000000000000000";

        /// <inheritdoc />
        public UserCreationDto GetExamples()
        {
            return new UserCreationDto
            {
                Email = "test@swaggerui.de",
                HashedPassword = "WInGB6lFMww1FZ2L7x1N17T2lPZwPiLre1Nl7+uTYx4=",
                EncryptedPrivateKey = "mWKSbPQay0Q1bCHTUNyr6rpUC3TzR7lFdtygsHbH5J1UzBnTooUFIAm17IUvuUqOGaajOnLShv/78zyXk7TCrF+/8NMtO+1+WDN7KXoMMwIV2YPYHxP/t/FwxDkhUXXukb3OLNZZq2mcFcWCDnHPNA8TFI33i54JAWoaGpC8Xv2xvm2dSBGQK7lN4QPRBqwJ42mQzS7eUtdssRLi3KFzozYrcjCaPdYyTxH8EFp8rYrNku5KgZycdwqvQkpXCo+wDguS1ygJaMyuEYl2PVImtJvZKUwHE3zo4DZXaSVbgKNuRtS4UlYp1MK/eBbdIVx2QQWIxIHE/7T6dlo6I/uoH3DLDwAkygaNMw+BzTQ8BuTYbzaYsPFwxK1wQ8y1vV7w48mOMrAORTMx6X4zTZJp7r5xsx3BubnhJd+BfGvmxz4A70ehNcqovXR3OYNGsys0RizjmsESP0QCCcHpINnFFE2RIxPOJ1MSp+pmih0G6al1gXiD2OfSYUpqcskxTO09sv8QsyJU13GhpwwyKlq4F32i3I7sfGju2W3DQkOL3hrBeNBGhR/+diIGZT3/KKcdIJp9UhWR2NojTBuUXRpVRUbUBTuYGgNN6lmbSXDGiWkCwmfqiCOCDOKrJ0E54l1E5FHU6VOjSMd5arFMuAQs1Gyyw7U2/gtTNUdbtN5oQUZzFAZNbAe9i8dmRLfJtvuZKG2YzA6QfDdeW/Ly32uKxbTiWzeZYS/vFQwskOgxhKm4GvVJJ6MhtHwXXiU6m4Tk1wvroAHzT0FsHyEbb1d46CuAVs+BcvmBw/SqAFnkqYW31Bhwffhar1Q7e/aRyRBdMzMelVS9RpDey/U3LuNneJAoi+1d5Y7V8OOklDJRlmxXBMJQRfEMfk0YufijPQpu1rO3g0AT+p14QF/iBXbyGlP9P4uhBoPzxEP5jWvAdMYOMGuzzOs58tvd92mbb6vErvymLPh41G/c9GZrsxI8dwV9ZKlKf2XS6aXIDlqsl1qGVdMXeQdC55ohsuqVmlAMrUhKwdlEPNQ+BogfnDAZ+lskd5epT7OfUzEdM7CPjER2UxChNxrvZGmw9bFw+3RT2hiDIh/c7W5WSboqcewXf3sgpmFgPKVHwbyk5QyNokz7bXhRqzkg8iI/lLtzTq0G3PLB5MEqgTHr1Zrwm8+GUD8v4yVD7IADZzBmCkLCDkGWiuOVKSL6cYMaydww3ofYEnCPcGPLfJClmyQZczgM2yCVgM5XoOPQr/h+bFJXVz9a4xzqGq9mhJdlwkxRTMXSJY7WNO16YPKJFXWv78EseMrYuI1QogL0IMimASKptHcH/hKtIxrXw7YQCs9bUtOp60BShe90RHtA0Jd85P/AQg==",
                PublicKey = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<RSAParameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <Exponent>AQAB</Exponent>\r\n  <Modulus>p1uEX6YqAWKKQVJaAv/m3o2w5xsa/3xdVwPujlRilwXjpjOYhlxiXctd382RNzzGjozNAhCJH3DKPrga81HeKPGuQXIDfHrZ7kVANt/bD+YzAiwuTTgK0MuNmAOFz3AVuenNfJ7RyzrbKR6eZSvn1wbi2HHrdHSZ9nthTJbROQliYjLedwX4ToU++mGyWTDTkP9MA4zAx/s+qeEnNMAn9Q7Mhy0m6TD2sdrBtptX4XIk7Hl62BlD10edmSv/xCbAVgzutkFzsKH+rwRJLR5KAwYRKGcOHnu6NhzFiHeeqHHe1vJ8fD3uytCLtwNwZFHvvS8rXhlq/Sk7hVFQeM76uQ==</Modulus>\r\n</RSAParameters>",
                Salt = "0000000000000000",
                UserId = "0146613259928c4bd8a58dfd0fca344e47"
            };
        }


    }
}
