using System;
using System.Text;
using System.Security.Cryptography;

using VentasGpo.iOS.Config;
using VentasGpo.Portable.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(DataEncrypt))]
namespace VentasGpo.iOS.Config
{
    public class DataEncrypt : IDataEncrypt
    {
        static string key = "01Nutrisa01";//ojo no tocar
        public string Cifrado(string source)
        {
            try
            {
                TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

                byte[] byteHash;
                byte[] byteBuff;

                byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
                desCryptoProvider.Key = byteHash;
                desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Encoding.UTF8.GetBytes(source);

                string encoded =
                    Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                encoded = encoded.Replace("=", "-");
                encoded = encoded.Replace("&", "$");
                encoded = encoded.Replace("+", "*");
                return encoded;
            }
            catch
            {
                return "";
            }
        }

        public string DesCifrado(string encodedText)
        {
            try
            {
                encodedText = encodedText.Replace("-", "=");
                encodedText = encodedText.Replace("$", "&");
                encodedText = encodedText.Replace("*", "+");
                TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

                byte[] byteHash;
                byte[] byteBuff;

                byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
                desCryptoProvider.Key = byteHash;
                desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Convert.FromBase64String(encodedText);

                string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                return plaintext;
            }
            catch
            {
                return "";
            }
        }
    }
}
