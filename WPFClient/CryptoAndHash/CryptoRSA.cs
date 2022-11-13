using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.CryptoAndHash
{
    public class CryptoRSA
    {

        private RSAParameters _privateKey;


        /// <summary>
        /// Method to Create a fresh public key for use by the server to encrypt the EAS keys and return them to client
        /// </summary>
        /// <returns>RSAParameters publicKey</returns>
        public RSAParameters AssignNewKey()
        {
            using (RSACryptoServiceProvider rsa = new())
            {
                rsa.PersistKeyInCsp = false;
                _privateKey = rsa.ExportParameters(true);
                RSAParameters publicKey = rsa.ExportParameters(false);
                return publicKey;
            }

        }


        /// <summary>
        /// Method to Decrypt the RSA encryption, to rethrive the AES keys
        /// </summary>
        /// <param name="dataToDecrypt"></param>
        /// <returns>byte[] decryptedData</returns>
        public byte[] DecryptRSA(byte[] dataToDecrypt)
        {
          
            byte[] decryptedData;

            using RSACryptoServiceProvider rsa = new();

            //Import key parameters into RSA.
            rsa.ImportParameters(_privateKey);

            decryptedData = rsa.Decrypt(dataToDecrypt, false);

            rsa.PersistKeyInCsp = false;
            
            return decryptedData;
        }


    }
}
