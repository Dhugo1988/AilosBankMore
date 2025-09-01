using APITarifa.Application.Common.Model;

namespace APITarifa.Application.Commands.CadastrarTarifa
{
    public class CadastrarTarifaResponse : BaseResponse
    {
        public string? TarifaId { get; set; }
        public string? IdContaCorrente { get; set; }
        public string? DataMovimento { get; set; }
        public decimal? Valor { get; set; }

        public CadastrarTarifaResponse(bool success, string? message = null, string? errorType = null)
            : base(success, message, errorType)
        {
        }

        public CadastrarTarifaResponse(bool success, string tarifaId, string idContaCorrente, string dataMovimento, decimal valor)
            : base(success)
        {
            TarifaId = tarifaId;
            IdContaCorrente = idContaCorrente;
            DataMovimento = dataMovimento;
            Valor = valor;
        }
    }
}
