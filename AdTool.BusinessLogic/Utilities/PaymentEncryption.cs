using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.BusinessLogic.Utilities
{
    public static class PaymentEncryption
    {
        public async static Task<string> EncryptString(string key, string plainText)
        {
            key += PaymentInfo.Crypty;


            using (Aes aes = Aes.Create())
            {
                aes.Key = DeriveKeyFromPassword(key);
                aes.IV = IV;

                using MemoryStream output = new();
                using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
                await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(plainText));
                await cryptoStream.FlushFinalBlockAsync();
                return Convert.ToBase64String(output.ToArray());
            }
        }

        public async static Task<string> DecryptString(string key, string cipherText)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);

            key += PaymentInfo.Crypty;

            using Aes aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(key);
            aes.IV = IV;
            using MemoryStream input = new(buffer);
            using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream output = new();
            await cryptoStream.CopyToAsync(output);
            return Encoding.Unicode.GetString(output.ToArray());
        }



        private static byte[] DeriveKeyFromPassword(string password)
        {
            var emptySalt = Array.Empty<byte>();
            var iterations = 1000;
            var desiredKeyLength = 16; // 16 bytes equal 128 bits.
            var hashMethod = HashAlgorithmName.SHA384;
            return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password),
                                             emptySalt,
                                             iterations,
                                             hashMethod,
                                             desiredKeyLength);
        }

        private static byte[] IV =
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
        };
    }
}
