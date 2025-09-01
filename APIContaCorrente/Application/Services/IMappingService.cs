using APIContaCorrente.Application.Commands.CadastrarContaCorrente;
using APIContaCorrente.Application.Commands.Login;
using APIContaCorrente.Application.Commands.Movimentar;
using APIContaCorrente.Application.Commands.InativarConta;
using APIContaCorrente.Application.DTOs;

namespace APIContaCorrente.Application.Services
{
    public interface IMappingService
    {
        CadastrarContaCorrenteCommand MapToCommand(CadastrarContaCorrenteDto dto);
        LoginCommand MapToCommand(LoginDto dto);
        MovimentarCommand MapToCommand(MovimentarDto dto, string contaCorrenteId);
        InativarContaCommand MapToCommand(InativarContaDto dto, string contaCorrenteId);
    }
}
