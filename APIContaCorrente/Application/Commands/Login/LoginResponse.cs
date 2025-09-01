using APIContaCorrente.Application.Common.Model;

namespace APIContaCorrente.Application.Commands.Login
{
    public class LoginResponse : BaseResponse
    {
        public string? Token { get; set; }

        public LoginResponse(bool success, string? message = null, string? errorType = null)
    : base(success, message, errorType)
        {
        }

        public LoginResponse(bool success, string? token = null) : base(success)
        {
            Token = token;
        }
    }
}
