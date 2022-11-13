using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.CryptoAndHash
{
    internal class CryptoAES
    {
        /// <summary>
        /// Create new instanse of AES
        /// </summary>
        /// <returns></returns>
        internal Aes CreateAES()
        {
            Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;

            return aes;
        }



        public byte[] EncryptAES(string input, byte[] key, byte[] iv)
        {
            try
            {
                // Checking arguments. (friendly lend to my by Microsoft, such a friendly company.)
                if (input == null || input.Length <= 0)
                    throw new ArgumentNullException("plainText is not valid");
                if (key == null || key.Length <= 0)
                    throw new ArgumentNullException("Key is not valid");
                if (iv == null || iv.Length <= 0)
                    throw new ArgumentNullException("IV is not valid");

                //Creating the incryptor
                using Aes aes = CreateAES();
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

                //Create streams
                using MemoryStream memoryStreamEncryptor = new();
                using CryptoStream cryptoStream = new(memoryStreamEncryptor, encryptor, CryptoStreamMode.Write);
                using (StreamWriter writer = new(cryptoStream))
                {
                    //Writing the data to the stream
                    writer.Write(input);
                }
                return memoryStreamEncryptor.ToArray();

            }
            catch (Exception)
            {
                throw;
            }
        }



        public string DecryptAes(byte[] input, byte[] key, byte[] iv)
        {
            try
            {
                // Checking arguments. (friendly lend to my by Microsoft, such a friendly company.)
                if (input == null || input.Length <= 0)
                    throw new ArgumentNullException("cipher text is not valid");
                if (key == null || key.Length <= 0)
                    throw new ArgumentNullException("Key is not valid");
                if (iv == null || iv.Length <= 0)
                    throw new ArgumentNullException("IV is not valid");

                //Creating the decryptor
                using Aes aes = CreateAES();
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);

                //Create streams
                using MemoryStream memoryStreamDecryptor = new(input);
                using CryptoStream cryptoStream = new(memoryStreamDecryptor, decryptor, CryptoStreamMode.Read);
                using StreamReader streamReaderDecrypt = new(cryptoStream);
                //Read the decrypted bytes, and adding them to a string
                return streamReaderDecrypt.ReadToEnd();
            }
            catch (Exception)
            {
                throw;
            }
        }




    }
}
