using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace SutesFozes.Infrastructure;

public class Argon2
{
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int DegreeOfParallelism = 8;
        private const int Iterations = 4;
        private const int MemorySize = 256 * 1024;

        public string HashPassword(string password, string saltString)
        {
                byte[] salt = Convert.FromBase64String(saltString);

                using(var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
                {
                        argon2.Salt = salt;
                        argon2.DegreeOfParallelism = DegreeOfParallelism;
                        argon2.MemorySize = MemorySize;
                        argon2.Iterations = Iterations;

                        byte[] hash = argon2.GetBytes(HashSize);
                        return Convert.ToBase64String(hash);
                }
        }

        public string GenerateSalt(int size = SaltSize)
        {
                using(var rng = RandomNumberGenerator.Create())
                {
                        byte[] salt = new byte[size];
                        rng.GetBytes(salt);
                        return Convert.ToBase64String(salt);
                }
        }
}