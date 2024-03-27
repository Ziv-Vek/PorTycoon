using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SupersonicWisdomSDK
{
    internal static class SwEncryptor
    {
        #region --- Constants ---
        
        private const int AES_ENCRYPTION_KEY_LENGTH = 32;

        #endregion
        
        
        #region --- Public Methods ---
        
        internal static string EncryptAesBase64(string textToEncrypt, string key, string iv)
        {
            if (textToEncrypt.SwIsNullOrEmpty()) return string.Empty;
            
            try
            {
                var encryptedText = EncryptAes(textToEncrypt, key, iv);
                
                return Convert.ToBase64String(encryptedText);
            }
            catch (Exception ex)
            {
                SwInfra.Logger?.LogError(EWisdomLogType.Encryptor, $"Failed to encrypt text: {ex.Message}");

                return null;
            }
        }

        internal static string DecryptAesBase64(string encryptedText, string key, string iv)
        {
            if (encryptedText.SwIsNullOrEmpty()) return string.Empty;
            
            try
            {
                var encryptedTextByteArray = Convert.FromBase64String(encryptedText);
                
                return DecryptAes(encryptedTextByteArray, key, iv);
            }
            catch (Exception ex)
            {
                SwInfra.Logger?.LogError(EWisdomLogType.Encryptor, $"Failed to decrypt text: {ex.Message}");

                return null;
            }
        }
        
        internal static byte[] EncryptAes(string textToEncrypt, string key, string iv)
        {
            if (!IsAesKeyValid(key) || textToEncrypt.SwIsNullOrEmpty() || iv.SwIsNullOrEmpty())
            {
                SwInfra.Logger?.LogError(EWisdomLogType.Encryptor, $"Failed to encrypt text: {textToEncrypt ?? "null"}, key: {key ?? "null"}, iv: {iv ?? "null"}");
                
                return null;
            }

            byte[] encrypted;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(textToEncrypt);
                        }

                        encrypted = ms.ToArray();
                    }
                }
            }

            return encrypted;
        }

        internal static string DecryptAes(byte[] encryptedText, string key, string iv)
        {
            if (!IsAesKeyValid(key) || encryptedText == null || iv.SwIsNullOrEmpty())
            {
                SwInfra.Logger?.LogError(EWisdomLogType.Encryptor, $"Failed to encrypt bytes: is null = {encryptedText == null}, key: {key ?? "null"}, iv: {iv ?? "null"}");
                
                return null;
            }
            
            string plaintext = null;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);;
                aes.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(encryptedText))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            plaintext = reader.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        #endregion


        #region --- Private Methods ---

        private static bool IsAesKeyValid(string key)
        {
            var isValid = key?.Length == AES_ENCRYPTION_KEY_LENGTH;

            if (!isValid)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Encryptor ,$"{key ?? "null"} length must be {AES_ENCRYPTION_KEY_LENGTH}");
            }
            
            return isValid;
        }

        #endregion
    }
}
