using Grpc.Core;

namespace BusinessSharkService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public async override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var username = context.GetHttpContext().User.Identity?.Name ?? "unknown";
            return await Task.FromResult(new HelloReply
            {
                Message = "Hello " + username + " from: " + request.Name
            });
        }
    }
}
