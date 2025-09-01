using APIContaCorrente.Application.Commands.CadastrarContaCorrente;
using APIContaCorrente.Application.Commands.Login;
using APIContaCorrente.Application.Commands.Movimentar;
using APIContaCorrente.Application.Commands.InativarConta;
using APIContaCorrente.Application.DTOs;

namespace APIContaCorrente.Application.Services
{
    public class MappingService : IMappingService
    {
        public CadastrarContaCorrenteCommand MapToCommand(CadastrarContaCorrenteDto dto)
        {
            return new CadastrarContaCorrenteCommand
            {
                Cpf = dto.Cpf,
                Nome = dto.Nome,
                Senha = dto.Senha
            };
        }

        public LoginCommand MapToCommand(LoginDto dto)
        {
            return new LoginCommand
            {
                Identificador = dto.Identificador,
                Senha = dto.Senha
            };
        }

        public MovimentarCommand MapToCommand(MovimentarDto dto, string contaCorrenteId)
        {
            return new MovimentarCommand
            {
                IdRequisicao = dto.IdRequisicao,
                Valor = dto.Valor,
                TipoMovimento = dto.TipoMovimento,
                ContaCorrenteId = contaCorrenteId,
                NumeroConta = dto.NumeroConta
            };
        }

        public InativarContaCommand MapToCommand(InativarContaDto dto, string contaCorrenteId)
        {
            return new InativarContaCommand
            {
                ContaCorrenteId = contaCorrenteId,
                Senha = dto.Senha
            };
        }
    }
}
