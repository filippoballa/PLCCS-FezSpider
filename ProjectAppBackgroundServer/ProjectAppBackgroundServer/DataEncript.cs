using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace ProjectAppBackgroundServer
{
    public class DataEncript
    {
        private const int MAX_DIM = 100;
        private byte[] IV = new byte[] { 0x26, 0xdc, 0xff, 
            0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

        private SymmetricAlgorithm aesObj;

        public DataEncript() 
        {
            this.aesObj = Rijndael.Create();
            byte[] plainKey = Encoding.ASCII.GetBytes("0123456789abcdef");
            aesObj.Key = plainKey;
            aesObj.Mode = CipherMode.CBC;
            aesObj.Padding = PaddingMode.Zeros;
            aesObj.IV = this.IV;
        }
       

        public string EncryptString(string plaintext)
        {
            
            byte[] plainTextBytes = new byte[DataEncript.MAX_DIM];
            plainTextBytes = Encoding.ASCII.GetBytes(plaintext);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aesObj.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(plainTextBytes, 0, plainTextBytes.Length);
            cs.Close();
            byte[] cipherBytes = ms.ToArray();
            ms.Close();

            return Convert.ToBase64String(cipherBytes);          
            
        }

        public string DecryptString(string ciphertext) 
        {            
            byte[] cipherBytes = new byte[MAX_DIM];
            cipherBytes = Convert.FromBase64String(ciphertext);
            MemoryStream ms = new MemoryStream(cipherBytes);
            CryptoStream cs = new CryptoStream(ms, this.aesObj.CreateDecryptor(), CryptoStreamMode.Read);
            cs.Read(cipherBytes, 0, cipherBytes.Length);
            string plaintext = Encoding.ASCII.GetString(ms.ToArray());
            ms.Close();
            cs.Close();
            return plaintext;            
        }
    }
}
