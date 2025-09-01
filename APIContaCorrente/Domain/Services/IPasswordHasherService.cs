namespace APIContaCorrente.Domain.Services
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password, out string salt);
        bool VerifyPassword(string password, string hash, string salt);
    }
}
