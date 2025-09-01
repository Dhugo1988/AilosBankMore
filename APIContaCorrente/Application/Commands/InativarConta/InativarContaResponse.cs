using APIContaCorrente.Application.Common.Model;

namespace APIContaCorrente.Application.Commands.InativarConta
{
    public class InativarContaResponse : BaseResponse
    {
        public InativarContaResponse(bool success, string? message = null, string? errorType = null)
            : base(success, message, errorType)
        {
        }

        public InativarContaResponse(bool success) : base(success)
        {
        }
    }
}
