using System.Security.Cryptography;
using System.Text;

namespace bacit_dotnet.MVC.Authentication
{
    public static class PassHash
    {
        //størrelse på salt
        private const int SaltSize = 32;
        //generere salt
        public static byte[] GenerateSalt()
        { 
            return RandomNumberGenerator.GetBytes(32); ;
        }

        public static byte[] ComputeHMAC_SHA256(byte[] data, byte[] salt)
        {
            using (var hmac = new HMACSHA256(salt))
            {
                return hmac.ComputeHash(data);
            }
        }
    }

}
