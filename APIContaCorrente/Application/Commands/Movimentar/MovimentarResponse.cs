using APIContaCorrente.Application.Common.Model;

namespace APIContaCorrente.Application.Commands.Movimentar
{
    public class MovimentarResponse : BaseResponse
    { 
        public MovimentarResponse(bool success, string? message = null, string? errorType = null)
            : base(success, message, errorType)
        {
        }

        public MovimentarResponse(bool success) : base(success)
        {
        }
    }
}
