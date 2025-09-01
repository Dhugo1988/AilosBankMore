using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using APITransferencia.Application.DTOs;
using APITransferencia.Application.Services;

namespace APITransferencia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferenciaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMappingService _mappingService;

        public TransferenciaController(IMediator mediator, IMappingService mappingService)
        {
            _mediator = mediator;
            _mappingService = mappingService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Efetuar([FromBody] EfetuarTransferenciaDto dto)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var contaCorrenteId = User.FindFirst("contaCorrenteId")?.Value;
            
            if (string.IsNullOrEmpty(contaCorrenteId)) return Forbid();
            
            var command = _mappingService.MapToCommand(dto, contaCorrenteId, authHeader);
            var resp = await _mediator.Send(command);
            
            if (!resp.Success)
            {
                return BadRequest(resp);
            }
            return NoContent();
        }
    }
}
