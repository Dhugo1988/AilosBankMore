using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APITarifa.Application.Commands.ConsultarTarifasPorConta;
using APITarifa.Application.Commands.ConsultarTarifaPorId;

namespace APITarifa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TarifaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TarifaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("conta/{idContaCorrente}")]
        public async Task<IActionResult> GetTarifasPorConta(string idContaCorrente)
        {
            var command = new ConsultarTarifasPorContaCommand { IdContaCorrente = idContaCorrente };
            var response = await _mediator.Send(command);

            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(response.Data);
        }

        [HttpGet("{idTarifa}")]
        public async Task<IActionResult> GetTarifa(string idTarifa)
        {
            var command = new ConsultarTarifaPorIdCommand { IdTarifa = idTarifa };
            var response = await _mediator.Send(command);

            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(response.Data);
        }
    }
}
