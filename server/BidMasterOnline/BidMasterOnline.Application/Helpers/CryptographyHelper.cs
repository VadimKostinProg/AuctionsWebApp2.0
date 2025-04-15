using System.Security.Cryptography;
using System.Text;

namespace BidMasterOnline.Application.Helpers
{
    public static class CryptographyHelper
    {
        /// <summary>
        /// Generates a salt as a randomly generated data of the fixed size.
        /// </summary>
        /// <param name="size">Size of salt(number of bytes).</param>
        /// <returns>Generated salt as string.</returns>
        public static string GenerateSalt(int size)
        {
            var cryptoProvider = new RNGCryptoServiceProvider();

            var bytes = new byte[size];

            cryptoProvider.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Computes hash of input plain string with salt.
        /// </summary>
        /// <param name="plainText">Plain text to hash.</param>
        /// <param name="salt">Salt to concat to the plain text for hashing.</param>
        /// <returns>Hashed input text.</returns>
        public static string Hash(string plainText, string salt)
        {
            using SHA256 sha256 = SHA256.Create();

            string textToHash = plainText + salt;

            var bytesToHash = Encoding.UTF8.GetBytes(textToHash);

            var hashedBytes = sha256.ComputeHash(bytesToHash);

            return Convert.ToBase64String(hashedBytes);
        }
    }
}
