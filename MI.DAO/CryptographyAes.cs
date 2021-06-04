using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MI.DAO
{
    public class CryptographyAes
    {
        private AesManaged _cryptoProvider = new AesManaged();
        private string _key;
        private string _vector;

        public CryptographyAes()
        {
            GenerateKeyTripleDes();
            AppSettingsReader AppConfig = new AppSettingsReader();
            _key = (string)AppConfig.GetValue("key", typeof(string));
            _vector = (string)AppConfig.GetValue("vector", typeof(string));
            _cryptoProvider.Key = HexToByte(_key);
            _cryptoProvider.IV = HexToByte(_vector);
        }

        public string EncryptString(string encryptValue)
        {
            byte[] valBytes = Encoding.Unicode.GetBytes(encryptValue);
            ICryptoTransform transform = _cryptoProvider.CreateEncryptor();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
            cs.Write(valBytes, 0, valBytes.Length);
            cs.FlushFinalBlock();
            byte[] returnBytes = ms.ToArray();
            cs.Close();
            return Convert.ToBase64String(returnBytes);
        }

        public string DecryptString(string encryptedValue)
        {
            byte[] valBytes = Convert.FromBase64String(encryptedValue);
            ICryptoTransform transform = _cryptoProvider.CreateDecryptor();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
            cs.Write(valBytes, 0, valBytes.Length);
            cs.FlushFinalBlock();
            byte[] returnBytes = ms.ToArray();
            cs.Close();
            return Encoding.Unicode.GetString(returnBytes);
        }

        private static string ByteToHex(byte[] byteArray)
        {
            string outString = "";
            foreach (
                Byte b in byteArray) outString += b.ToString("X2");
            return outString;
        }

        private static byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// Generar un nuevo key y vector
        /// </summary>
        /// <returns></returns>
        private string GenerateKeyTripleDes()
        {
            string NuevoKeyVector = string.Empty;

            _cryptoProvider.GenerateIV();
            _cryptoProvider.GenerateKey();

            NuevoKeyVector = ByteToHex(_cryptoProvider.Key) + "*" + ByteToHex(_cryptoProvider.IV);

            return NuevoKeyVector;
        }
    }
}
