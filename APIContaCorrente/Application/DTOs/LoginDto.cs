using System.ComponentModel.DataAnnotations;

namespace APIContaCorrente.Application.DTOs
{
    public class LoginDto
    {
        public string Identificador { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
