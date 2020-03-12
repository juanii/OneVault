using System;
using System.IO;
using System.Security.Cryptography;

namespace OneVault.OPVault
{
    public class KeyPair
    {
        public byte[] encryptionKey { get; set; }

        public byte[] macKey { get; set; }
    }

    public static class Crypto
    {
        public const int IV_SIZE = 16;

        public static KeyPair HashKey(byte[] data, KeyPair keyPair)
        {
            byte[] keyHash;

            using (SHA512 sha512 = SHA512.Create())
                keyHash = sha512.ComputeHash(data);

            byte[] encryptionKey = new byte[keyHash.Length / 2];
            byte[] macKey = new byte[keyHash.Length / 2];

            Array.Copy(keyHash, 0, encryptionKey, 0, encryptionKey.Length);
            Array.Copy(keyHash, encryptionKey.Length, macKey, 0, macKey.Length);

            return new KeyPair() { encryptionKey = encryptionKey, macKey = macKey };
        }

        public static KeyPair DeriveKey(byte[] key, byte[] salt, int iterations)
        {
            byte[] derivedKey;

            using (HMACSHA512 hmac = new HMACSHA512())
            {
                Medo.Security.Cryptography.Pbkdf2 pbkdf2 = new Medo.Security.Cryptography.Pbkdf2(hmac, key, salt, iterations);
                derivedKey = pbkdf2.GetBytes(hmac.HashSize / 8);
            }

            byte[] derivedEncryptionKey = new byte[derivedKey.Length / 2];
            byte[] derivedMACKey = new byte[derivedKey.Length / 2];

            Array.Copy(derivedKey, 0, derivedEncryptionKey, 0, derivedEncryptionKey.Length);
            Array.Copy(derivedKey, derivedEncryptionKey.Length, derivedMACKey, 0, derivedMACKey.Length);

            return new KeyPair() { encryptionKey = derivedEncryptionKey, macKey = derivedMACKey };
        }

        static public byte[] Decrypt(ArraySegment<byte> encryptedBlob, ArraySegment<byte> iv, byte[] encryptionKey)
        {
            using (Rijndael rijndael = Rijndael.Create())
            {
                rijndael.Key = encryptionKey;
                rijndael.IV = ArraySegmentExt.GetByteArray(iv);
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.None;

                using (ICryptoTransform decryptor = rijndael.CreateDecryptor())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(encryptedBlob.Array, encryptedBlob.Offset, encryptedBlob.Count);
                    return ms.ToArray();
                }
            }
        }
    }
}
