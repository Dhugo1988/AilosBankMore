using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using APIContaCorrente.Application.Queries.Saldo;
using APIContaCorrente.Application.DTOs;
using APIContaCorrente.Application.Services;
using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Application.Queries.ConsultarContaCorrente;

namespace APIContaCorrente.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMappingService _mappingService;

        public ContaCorrenteController(IMediator mediator, IMappingService mappingService)
        {
            _mediator = mediator;
            _mappingService = mappingService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CadastrarContaCorrente([FromBody] CadastrarContaCorrenteDto dto)
        {
            var command = _mappingService.MapToCommand(dto);
            var response = await _mediator.Send(command);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var command = _mappingService.MapToCommand(dto);
            var response = await _mediator.Send(command);
            
            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        [HttpPost("inativar")]
        [Authorize]
        public async Task<IActionResult> Inativar([FromBody] InativarContaDto dto)
        {
            var contaCorrenteId = User.FindFirst("contaCorrenteId")?.Value;
            if (string.IsNullOrEmpty(contaCorrenteId)) return Forbid();
            
            var command = _mappingService.MapToCommand(dto, contaCorrenteId);
            var response = await _mediator.Send(command);
            
            if (!response.Success)
            {
                if (response.ErrorType is ValidationConstants.ERROR_USER_UNAUTHORIZED)
                {
                    return Unauthorized(response);
                }
    
                return BadRequest(response);
            }
            return NoContent();
        }

        [HttpPost("movimentar")]
        [Authorize]
        public async Task<IActionResult> Movimentar([FromBody] MovimentarDto dto)
        {
            var contaId = User.FindFirst("contaCorrenteId")?.Value;
            if (string.IsNullOrEmpty(contaId)) return Forbid();

            var contaNumero = User.FindFirst("accountNumber")?.Value;
            if (string.IsNullOrEmpty(contaId)) return Forbid();

            dto.NumeroConta ??= int.TryParse(contaNumero, out int numero) ? numero : null;

            var command = _mappingService.MapToCommand(dto, contaId);
            var response = await _mediator.Send(command);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return NoContent();
        }

        [HttpGet("saldo")]
        [Authorize]
        public async Task<IActionResult> Saldo()
        {
            var contaCorrenteId = User.FindFirst("contaCorrenteId")?.Value;
            if (string.IsNullOrEmpty(contaCorrenteId)) return Forbid();
            
            var response = await _mediator.Send(new SaldoQuery { ContaCorrenteId = contaCorrenteId });
            
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("por-numero")]
        [Authorize]
        public async Task<IActionResult> ConsultarContaCorrenteId([FromQuery] string? cpf = null, [FromQuery] int? numeroConta = null)
        {
            var query = new ConsultarContaCorrenteQuery { Cpf = cpf, NumeroConta = numeroConta };
            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("por-id")]
        [Authorize]
        public async Task<IActionResult> ConsultarContaCorrenteNumero([FromQuery] string? contaId = null)
        {
            var query = new ConsultarContaCorrenteNumeroQuery { ContaId = contaId };
            var response = await _mediator.Send(query);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
