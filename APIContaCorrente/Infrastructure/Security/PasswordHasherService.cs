using System.Security.Cryptography;
using APIContaCorrente.Domain.Services;

namespace APIContaCorrente.Infrastructure.Security
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string password, out string salt)
        {
            // Gerar salt aleatório
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);

            // Hash da senha com PBKDF2
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            try
            {
                byte[] saltBytes = Convert.FromBase64String(salt);
                byte[] storedHash = Convert.FromBase64String(hash);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] computedHash = pbkdf2.GetBytes(32);
                    return storedHash.SequenceEqual(computedHash);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
