using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace eNetwork3.Utils
{
    /// <summary>
    /// Utils class with useful fonctions
    /// </summary>
    public static class eUtils
    {
        /// <summary>
        /// Encrypt the buffer
        /// </summary>
        /// <param name="buffer">Buffer to encrypt</param>
        /// <param name="key">Encryption key</param>
        /// <returns>Encrypted buffer</returns>
        public static byte[] Encrypt(byte[] buffer, string key)
        {
            PasswordDeriveBytes pdb =
              new PasswordDeriveBytes(key,
              new byte[] { 0x53, 0x6f, 0x64, 0x69, 0x75, 0x6d, 0x20, 0x43, 0x68, 0x6c, 0x6f, 0x72, 0x69, 0x64, 0x65 });
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms,
              aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(buffer, 0, buffer.Length);
            cs.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// Decrypt the buffer
        /// </summary>
        /// <param name="buffer">Buffer to decrypt</param>
        /// <param name="key">Decryption key</param>
        /// <returns>Decrypted buffer</returns>
        public static byte[] Decrypt(byte[] buffer, string key)
        {
            PasswordDeriveBytes pdb =
              new PasswordDeriveBytes(key, // Change this
              new byte[] { 0x53, 0x6f, 0x64, 0x69, 0x75, 0x6d, 0x20, 0x43, 0x68, 0x6c, 0x6f, 0x72, 0x69, 0x64, 0x65 });
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms,
              aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(buffer, 0, buffer.Length);
            cs.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// Compress a buffer
        /// </summary>
        /// <param name="buffer">Buffer to compress</param>
        /// <returns>Compressed buffer</returns>
        public static byte[] Compress(byte[] buffer)
        {
            if (buffer == null)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(ms, CompressionMode.Compress))
                {
                    gs.Write(buffer, 0, buffer.Length);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decompress a buffer
        /// </summary>
        /// <param name="buffer">Buffer to decompress</param>
        /// <returns>Decompressed buffer</returns>
        public static byte[] Decompress(byte[] buffer)
        {
            if (buffer == null)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(ms, CompressionMode.Decompress))
                {
                    gs.Write(buffer, 0, buffer.Length);
                }
                return ms.ToArray();
            }
        }
    }
}
