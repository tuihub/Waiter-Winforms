using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;

namespace Waiter.Interceptors
{
    /// <summary>
    /// Client interceptor that handles JWT token management.
    /// Automatically adds AccessToken to requests and refreshes tokens when needed.
    /// Based on the Sentinel-CSharp implementation.
    /// </summary>
    public class ClientTokenInterceptor : Interceptor
    {
        private readonly ILogger<ClientTokenInterceptor> _logger;
        private readonly Services.TokenService _tokenService;
        private readonly Services.ConfigService _configService;

        public ClientTokenInterceptor(
            ILogger<ClientTokenInterceptor> logger,
            Services.TokenService tokenService,
            Services.ConfigService configService)
        {
            _logger = logger;
            _tokenService = tokenService;
            _configService = configService;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            CancellationToken ct = context.Options.CancellationToken;
            try
            {
                if (string.IsNullOrWhiteSpace(_tokenService.AccessToken))
                {
                    _logger.LogWarning("No access token found, refreshing token");
                    var tokens = RefreshTokenAsync(_tokenService.RefreshToken, ct).Result;
                    _tokenService.SetTokens(tokens.accessToken, tokens.refreshToken);
                }

                return ContinueWithAccessToken(request, context, continuation);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogInformation("Received unauthenticated error, refreshing token and retrying");

                var tokens = RefreshTokenAsync(_tokenService.RefreshToken, ct).Result;
                _tokenService.SetTokens(tokens.accessToken, tokens.refreshToken);

                return ContinueWithAccessToken(request, context, continuation);
            }
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            throw new NotImplementedException("BlockingUnaryCall is not implemented. Use async calls instead.");
        }

        private async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken, CancellationToken ct)
        {
            _logger.LogInformation("Refreshing token");
            var headers = new Metadata
            {
                { "Authorization", $"Bearer {refreshToken}" }
            };
            var client = new LibrarianSephirahService.LibrarianSephirahServiceClient(GrpcChannel.ForAddress(_configService.ServerUrl));
            var token = await client.RefreshTokenAsync(new RefreshTokenRequest(), headers, cancellationToken: ct);
            ct.ThrowIfCancellationRequested();
            return (token.AccessToken, token.RefreshToken);
        }

        private AsyncUnaryCall<TResponse> ContinueWithAccessToken<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
            where TRequest : class
            where TResponse : class
        {
            var metadata = context.Options.Headers ?? new Metadata();
            var authMetadata = metadata.SingleOrDefault(x => x.Key == "authorization");
            if (authMetadata != null)
            {
                metadata.Remove(authMetadata);
            }
            metadata.Add("Authorization", $"Bearer {_tokenService.AccessToken}");

            var newOptions = context.Options.WithHeaders(metadata);
            var newContext = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, newOptions);

            return base.AsyncUnaryCall(request, newContext, continuation);
        }
    }

    /// <summary>
    /// Interceptor that adds DownloadToken for file download operations.
    /// </summary>
    public class DownloadTokenInterceptor : Interceptor
    {
        private readonly Services.TokenService _tokenService;

        public DownloadTokenInterceptor(Services.TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var metadata = context.Options.Headers ?? new Metadata();
            if (!string.IsNullOrWhiteSpace(_tokenService.DownloadToken))
            {
                var authMetadata = metadata.SingleOrDefault(x => x.Key == "authorization");
                if (authMetadata != null)
                {
                    metadata.Remove(authMetadata);
                }
                metadata.Add("Authorization", $"Bearer {_tokenService.DownloadToken}");
            }

            var newOptions = context.Options.WithHeaders(metadata);
            var newContext = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, newOptions);

            return base.AsyncUnaryCall(request, newContext, continuation);
        }
    }
}
