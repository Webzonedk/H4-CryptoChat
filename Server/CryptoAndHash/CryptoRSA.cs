using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server.CryptoAndHash
{
    public class CryptoRSA
    {


        //private byte[] publicKey;
        private RSAParameters _privateKey;

        internal byte[] AssignNewKey()
        {
            using (RSACryptoServiceProvider rsa = new(2048))
            {
                rsa.PersistKeyInCsp = false;
                _privateKey = rsa.ExportParameters(true);
                return rsa.ExportRSAPublicKey();
            }

        }



        //Encrypting method to encrypt with a foreign public key
        public (byte[], byte[]) EncryptRSA(byte[] publicKey)
        {

            byte[] Exponent = { 1, 0, 1 };

            //Values to store encrypted symmetric keys.
            byte[] EncryptedSymmetricKey;
            byte[] EncryptedSymmetricIV;

            using RSACryptoServiceProvider rsa = new();

            RSAParameters RSAKeyInfo = new RSAParameters();

            //Set RSAKeyInfo to the public key values. 
            RSAKeyInfo.Modulus = publicKey;
            RSAKeyInfo.Exponent = Exponent;

            //Import key parameters into RSA.
            rsa.ImportParameters(RSAKeyInfo);

            //Create a new instance of the Aes class.
            Aes aes = Aes.Create();

            //Encrypt the symmetric key and IV.
            EncryptedSymmetricKey = rsa.Encrypt(aes.Key, false);
            EncryptedSymmetricIV = rsa.Encrypt(aes.IV, false);


            return (EncryptedSymmetricKey, EncryptedSymmetricIV);

        }


        ////Encrypting method to ecrypt with a foreign public key
        //public byte[] EncryptOwnRSA(byte[] dataToEncrypt)
        //{
        //    byte[] cipherbytes;

        //    using (RSACryptoServiceProvider rsa = new(2048))
        //    {
        //        rsa.PersistKeyInCsp = false;

        //        rsa.ImportParameters(_publicKey);

        //        cipherbytes = rsa.Encrypt(dataToEncrypt, true);
        //    }

        //    return cipherbytes;
        //}



        public byte[] DecryptRSA(byte[] dataToDecrypt)
        {
            byte[] plainText;

            using (RSACryptoServiceProvider rsa = new(2048))
            {
                rsa.PersistKeyInCsp = false;

                rsa.ImportParameters(_privateKey);
                plainText = rsa.Decrypt(dataToDecrypt, true);
            }

            return plainText;//Still needs to be converted to base64 string
        }


    }
}
