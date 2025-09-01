namespace APITarifa.Application.Common.Model
{
    public abstract class BaseResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorType { get; set; }

        protected BaseResponse(bool success, string? message = null, string? errorType = null)
        {
            Success = success;
            Message = message;
            ErrorType = errorType;
        }

        protected BaseResponse(bool success)
        {
            Success = success;
        }
    }
}
