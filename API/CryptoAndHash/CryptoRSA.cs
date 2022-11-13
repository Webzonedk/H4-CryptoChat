using System.Security.Cryptography;
using API.CryptoAndHash;

namespace API.CryptoAndHash
{
    public class CryptoRSA
    {
        /// <summary>
        /// Method to encrypt the AES keys 
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="aes"></param>
        /// <returns>byte[] EncryptedSymmetricKey, byte[] EncryptedSymmetricIV</returns>
        public (byte[], byte[]) EncryptRSA(byte[] publicKey, Aes aes)
        {
            using RSACryptoServiceProvider rsa = new();

            RSAParameters RSAKeyInfo = rsa.ExportParameters(false);
            {
                //Set RSAKeyInfo to the public key values. 
                RSAKeyInfo.Modulus = publicKey;
            };

            //Import key parameters into RSA.
            rsa.ImportParameters(RSAKeyInfo);

            //Encrypt the symmetric key and IV.
            byte[] EncryptedSymmetricKey = rsa.Encrypt(aes.Key, false);
            byte[] EncryptedSymmetricIV = rsa.Encrypt(aes.IV, false);

            return (EncryptedSymmetricKey, EncryptedSymmetricIV);
        }
    }
}
