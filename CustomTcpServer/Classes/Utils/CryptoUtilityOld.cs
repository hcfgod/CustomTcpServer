namespace InfinityServer.Classes.Utils
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    namespace InfinityServer.Classes.Utils
    {
        public static class CryptoUtilityOld
        {
            private static byte[] _aesKey; // 32 bytes for AES-256
            private static byte[] _aesIV; // 16 bytes for AES-256

            public static byte[] AesKey => _aesKey;
            public static byte[] AesIV => _aesIV;

            public static void Initialize()
            {
                _aesKey = GenerateRandomKey();
                _aesIV = GenerateRandomIV();
            }
            public static byte[] AesEncrypt(byte[] data)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = _aesKey;
                    aesAlg.IV = _aesIV;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(data, 0, data.Length);
                        }

                        return msEncrypt.ToArray();
                    }
                }
            }

            public static byte[] AesDecrypt(byte[] encryptedData)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = _aesKey;
                    aesAlg.IV = _aesIV;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            byte[] decryptedData = new byte[encryptedData.Length];
                            int bytesRead = csDecrypt.Read(decryptedData, 0, decryptedData.Length);

                            Array.Resize(ref decryptedData, bytesRead);
                            return decryptedData;
                        }
                    }
                }
            }

            public static byte[] RsaEncrypt(byte[] data, RSAParameters rsaParameters)
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportParameters(rsaParameters);
                    return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
                }
            }

            public static byte[] RsaDecrypt(byte[] encryptedData, RSAParameters rsaParameters)
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportParameters(rsaParameters);
                    return rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
                }
            }
            private static byte[] GenerateRandomKey()
            {
                byte[] key = new byte[32]; // 32 bytes

                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(key);
                }

                return key;
            }

            private static byte[] GenerateRandomIV()
            {
                byte[] iv = new byte[16]; // 16 bytes

                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(iv);
                }

                return iv;
            }
        }
    }

}
