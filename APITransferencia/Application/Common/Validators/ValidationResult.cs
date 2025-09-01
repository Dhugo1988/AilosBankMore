//namespace APITransferencia.Application.Common.Validators
//{
//    public class ValidationResult<T> where T : class
//    {
//        public bool IsValid { get; private set; }
//        public T ErrorResponse { get; private set; }

//        protected ValidationResult(bool isValid, T errorResponse = null)
//        {
//            IsValid = isValid;
//            ErrorResponse = errorResponse;
//        }
//        public static ValidationResult<T> Valid() => new(true);
//        public static ValidationResult<T> Invalid(T errorResponse) => new(false, errorResponse);
//    }
//}
