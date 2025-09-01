using APITransferencia.Application.Common.Model;

namespace APITransferencia.Application.Commands.EfetuarTransferencia
{
    public class EfetuarTransferenciaResponse : BaseResponse
    {
        public EfetuarTransferenciaResponse(bool success, string? message = null, string? errorType = null)
          : base(success, message, errorType)
        {
        }

        public EfetuarTransferenciaResponse(bool success) : base(success)
        {
        }
    }
}
