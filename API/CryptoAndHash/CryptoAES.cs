using System.Security.Cryptography;

namespace API.CryptoAndHash
{
    public class CryptoAES
    {
        /// <summary>
        /// Create new instanse of AES
        /// </summary>
        /// <returns></returns>
        public Aes CreateAES()
        {
            Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;

            return aes;
        }
    }
}
