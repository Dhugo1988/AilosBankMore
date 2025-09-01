using APIContaCorrente.Domain.Services;

namespace APIContaCorrente.Infrastructure.Services
{
    public class ContaCorrenteService : IContaCorrenteService
    {
        public int GerarNumeroConta()
        {
            // Gerar número de conta de 8 dígitos
            var random = new Random();
            return random.Next(10000000, 99999999);
        }
    }
}
