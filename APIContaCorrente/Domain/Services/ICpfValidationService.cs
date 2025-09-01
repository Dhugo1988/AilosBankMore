namespace APIContaCorrente.Domain.Services
{
    public interface ICpfValidationService
    {
        bool IsValid(string cpf);
        string CleanCpf(string cpf);
    }
}
