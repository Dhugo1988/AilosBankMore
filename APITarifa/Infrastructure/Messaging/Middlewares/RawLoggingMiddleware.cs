//using KafkaFlow;
//using System.Text;

//namespace APITarifa.Infrastructure.Messaging.Middlewares
//{
//    public class RawLoggingMiddleware : IMessageMiddleware
//    {
//        private readonly ILogger<RawLoggingMiddleware> _logger;

//        public RawLoggingMiddleware(ILogger<RawLoggingMiddleware> logger)
//        {
//            _logger = logger;
//            _logger.LogInformation("🔧 RawLoggingMiddleware registrado");
//        }

//        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
//        {
//            var value = context.Message.Value;
//            var typeName = value != null ? value.GetType().FullName : "<null>";
//            _logger.LogInformation("🔎 Middleware: mensagem recebida do tipo {Type}", typeName);

//            if (value is byte[] bytes)
//            {
//                var raw = Encoding.UTF8.GetString(bytes);
//                _logger.LogInformation("🧾 Payload (raw): {Raw}", raw);
//            }
//            else
//            {
//                _logger.LogInformation("🧾 Payload (obj): {Obj}", value);
//            }

//            await next(context);
//        }
//    }
//}


