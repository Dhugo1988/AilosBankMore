namespace APIContaCorrente.Application.Common.Model
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; } = null;
        public string? ErrorType { get; set; } = null;

        public BaseResponse(bool success, string? message = null, string? errorType = null)
        {
            Success = success;
            Message = message;
            ErrorType = errorType;
        }
        public BaseResponse(bool success)
        {
            Success = success;
        }
    }
}
