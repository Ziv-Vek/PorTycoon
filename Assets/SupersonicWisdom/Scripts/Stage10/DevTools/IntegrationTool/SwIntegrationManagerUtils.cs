#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    public static class SwIntegrationManagerUtils
    {
        #region --- Members ---

        private static Action<string> _onDone;

        #endregion


        #region --- Public Methods ---

        public static IEnumerator GetIntegrationData(IntegrationLink integrationLink, Action<string> callback)
        {
            yield return Download(integrationLink, callback);
        }

        #endregion


        #region --- Private Methods ---

        private static string DecryptStringFromBase64(string cipherText, string Key, string IV)
        {
            // Check arguments
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }

            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }

            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            // Convert Base64 strings to byte arrays
            var cipherTextBytes = Convert.FromBase64String(cipherText);
            var KeyBytes = Convert.FromBase64String(Key);
            var IVBytes = Convert.FromBase64String(IV);
            // Declare the string used to hold the decrypted text
            string plaintext = null;

            // Create an Aes object with the specified key and IV
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = KeyBytes;
                aesAlg.IV = IVBytes;
                // Create a decryptor to perform the stream transform
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption
                using (var msDecrypt = new MemoryStream(cipherTextBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and place them in a string
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        private static IEnumerator Download(IntegrationLink integrationLink, Action<string> callback)
        {
            var client = new SwUnityWebRequestClient();
            var response = new SwWebResponse();

            var integrationData = string.Empty;

            yield return client.Get(integrationLink.url, response, 2000);

            if (response.error != SwWebRequestError.None)
            {
                SwInfra.Logger.LogError(EWisdomLogType.IntegrationTool, "Error downloading JSON: " + response.error);
            }
            else
            {
                integrationData = JsonConvert.DeserializeObject<SwIntegrationResponse>(response?.Text ?? string.Empty)?.data ?? string.Empty;
            }

            callback?.Invoke(DecryptStringFromBase64(integrationData, integrationLink.key, integrationLink.iv));
        }

        #endregion
    }

    internal class SwIntegrationResponse
    {
        #region --- Members ---

        public string data;

        #endregion
    }
}
#endif