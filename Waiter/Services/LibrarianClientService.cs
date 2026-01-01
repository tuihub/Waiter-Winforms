using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;
using Waiter.Interceptors;

namespace Waiter.Services
{
    /// <summary>
    /// Main client service for communicating with the Librarian server.
    /// Provides access to all Sephirah services.
    /// </summary>
    public class LibrarianClientService : IDisposable
    {
        private readonly ILogger<LibrarianClientService> _logger;
        private readonly TokenService _tokenService;
        private readonly ConfigService _configService;
        private readonly ILoggerFactory _loggerFactory;
        private GrpcChannel? _channel;
        private GrpcChannel? _anonymousChannel;
        private GrpcChannel? _downloadChannel;
        private CallInvoker? _callInvoker;
        private bool _disposed;

        public LibrarianClientService(
            ILogger<LibrarianClientService> logger,
            TokenService tokenService,
            ConfigService configService,
            ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _tokenService = tokenService;
            _configService = configService;
            _loggerFactory = loggerFactory;

            _configService.ConfigChanged += OnConfigChanged;
            InitializeChannels();
        }

        private void OnConfigChanged(object? sender, EventArgs e)
        {
            InitializeChannels();
        }

        private void InitializeChannels()
        {
            // Dispose old channels
            _channel?.Dispose();
            _anonymousChannel?.Dispose();
            _downloadChannel?.Dispose();

            // Create new channels
            _channel = GrpcChannel.ForAddress(_configService.ServerUrl);
            _anonymousChannel = GrpcChannel.ForAddress(_configService.ServerUrl);
            _downloadChannel = GrpcChannel.ForAddress(_configService.ServerUrl);

            var interceptor = new ClientTokenInterceptor(
                _loggerFactory.CreateLogger<ClientTokenInterceptor>(),
                _tokenService,
                _configService);

            _callInvoker = _channel.Intercept(interceptor);
        }

        /// <summary>
        /// Gets a client without token interceptor for login operations.
        /// Uses a reusable channel to avoid resource leaks.
        /// </summary>
        public LibrarianSephirahService.LibrarianSephirahServiceClient GetAnonymousClient()
        {
            if (_anonymousChannel == null)
            {
                InitializeChannels();
            }
            return new LibrarianSephirahService.LibrarianSephirahServiceClient(_anonymousChannel);
        }

        /// <summary>
        /// Gets an authenticated client with token interceptor.
        /// </summary>
        public LibrarianSephirahService.LibrarianSephirahServiceClient GetAuthenticatedClient()
        {
            if (_callInvoker == null)
            {
                InitializeChannels();
            }
            return new LibrarianSephirahService.LibrarianSephirahServiceClient(_callInvoker);
        }

        /// <summary>
        /// Gets a client with download token for file download operations.
        /// Uses a reusable channel to avoid resource leaks.
        /// </summary>
        public LibrarianSephirahService.LibrarianSephirahServiceClient GetDownloadClient()
        {
            if (_downloadChannel == null)
            {
                InitializeChannels();
            }
            var interceptor = new DownloadTokenInterceptor(_tokenService);
            return new LibrarianSephirahService.LibrarianSephirahServiceClient(_downloadChannel!.Intercept(interceptor));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _configService.ConfigChanged -= OnConfigChanged;
                    _channel?.Dispose();
                    _anonymousChannel?.Dispose();
                    _downloadChannel?.Dispose();
                }
                _disposed = true;
            }
        }

        #region Tiphereth - Authentication and User Management

        public async Task<(string accessToken, string refreshToken)> LoginAsync(string username, string password)
        {
            _logger.LogInformation("Logging in user: {Username}", username);
            var client = GetAnonymousClient();
            var request = new GetTokenRequest
            {
                Username = username,
                Password = password
            };
            var response = await client.GetTokenAsync(request);
            _tokenService.SetTokens(response.AccessToken, response.RefreshToken);
            return (response.AccessToken, response.RefreshToken);
        }

        public async Task<(string accessToken, string refreshToken)?> RefreshTokenAsync()
        {
            try
            {
                _logger.LogInformation("Refreshing token");
                var client = GetAnonymousClient();
                var headers = new Grpc.Core.Metadata
                {
                    { "Authorization", $"Bearer {_tokenService.RefreshToken}" }
                };
                var response = await client.RefreshTokenAsync(new RefreshTokenRequest(), headers);
                _tokenService.SetTokens(response.AccessToken, response.RefreshToken);
                return (response.AccessToken, response.RefreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh token");
                return null;
            }
        }

        public void Logout()
        {
            _tokenService.ClearTokens();
            _logger.LogInformation("User logged out");
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            try
            {
                var client = GetAuthenticatedClient();
                var response = await client.GetUserAsync(new GetUserRequest());
                return response.User;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current user");
                return null;
            }
        }

        public async Task<ServerInformation?> GetServerInformationAsync()
        {
            try
            {
                var client = GetAnonymousClient();
                var response = await client.GetServerInformationAsync(new GetServerInformationRequest());
                return response.ServerInformation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get server information");
                return null;
            }
        }

        #endregion

        #region Gebura - App Management

        public async Task<ListAppsResponse?> ListAppsAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListAppsRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListAppsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list apps");
                return null;
            }
        }

        public async Task<TuiHub.Protos.Librarian.V1.InternalID?> CreateAppAsync(App app)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new CreateAppRequest { App = app };
                var response = await client.CreateAppAsync(request);
                return response.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create app");
                return null;
            }
        }

        public async Task<bool> UpdateAppAsync(App app)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdateAppRequest { App = app };
                await client.UpdateAppAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update app");
                return false;
            }
        }

        // Note: DeleteApp API is not available in the current proto version
        // This is left as a placeholder for future implementation
        public Task<bool> DeleteAppAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            _logger.LogWarning("DeleteApp API is not available in the current proto version");
            return Task.FromResult(false);
        }

        public async Task<SearchStoreAppsResponse?> SearchStoreAppsAsync(string nameLike, int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new SearchStoreAppsRequest
                {
                    NameLike = nameLike,
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.SearchStoreAppsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search store apps");
                return null;
            }
        }

        public async Task<GetStoreAppSummaryResponse?> GetStoreAppSummaryAsync(TuiHub.Protos.Librarian.V1.InternalID storeAppId)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new GetStoreAppSummaryRequest { StoreAppId = storeAppId };
                return await client.GetStoreAppSummaryAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get store app summary");
                return null;
            }
        }

        public async Task<TuiHub.Protos.Librarian.V1.InternalID?> AcquireStoreAppAsync(TuiHub.Protos.Librarian.V1.InternalID storeAppId)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new AcquireStoreAppRequest { StoreAppId = storeAppId };
                var response = await client.AcquireStoreAppAsync(request);
                return response.AppId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to acquire store app");
                return null;
            }
        }

        #endregion

        #region Gebura - App Category Management

        public async Task<ListAppCategoriesResponse?> ListAppCategoriesAsync()
        {
            try
            {
                var client = GetAuthenticatedClient();
                return await client.ListAppCategoriesAsync(new ListAppCategoriesRequest());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list app categories");
                return null;
            }
        }

        public async Task<TuiHub.Protos.Librarian.V1.InternalID?> CreateAppCategoryAsync(AppCategory category)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new CreateAppCategoryRequest { AppCategory = category };
                var response = await client.CreateAppCategoryAsync(request);
                return response.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create app category");
                return null;
            }
        }

        public async Task<bool> UpdateAppCategoryAsync(AppCategory category)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdateAppCategoryRequest { AppCategory = category };
                await client.UpdateAppCategoryAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update app category");
                return false;
            }
        }

        public async Task<bool> DeleteAppCategoryAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new DeleteAppCategoryRequest { Id = id };
                await client.DeleteAppCategoryAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete app category");
                return false;
            }
        }

        #endregion

        #region Gebura - App Save File Management

        public async Task<string?> UploadAppSaveFileAsync(TuiHub.Protos.Librarian.V1.InternalID appId, TuiHub.Protos.Librarian.V1.FileMetadata fileMetadata)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UploadAppSaveFileRequest
                {
                    AppId = appId,
                    FileMetadata = fileMetadata
                };
                var response = await client.UploadAppSaveFileAsync(request);
                return response.UploadToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload app save file");
                return null;
            }
        }

        public async Task<string?> DownloadAppSaveFileAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new DownloadAppSaveFileRequest { Id = id };
                var response = await client.DownloadAppSaveFileAsync(request);
                return response.DownloadToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download app save file");
                return null;
            }
        }

        public async Task<ListAppSaveFilesResponse?> ListAppSaveFilesAsync(TuiHub.Protos.Librarian.V1.InternalID appId)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListAppSaveFilesRequest { AppId = appId };
                return await client.ListAppSaveFilesAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list app save files");
                return null;
            }
        }

        public async Task<bool> DeleteAppSaveFileAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new DeleteAppSaveFileRequest { Id = id };
                await client.DeleteAppSaveFileAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete app save file");
                return false;
            }
        }

        #endregion

        #region Gebura - App RunTime Management

        public async Task<bool> BatchCreateAppRunTimeAsync(IEnumerable<AppRunTime> runTimes)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new BatchCreateAppRunTimeRequest();
                request.AppRunTimes.AddRange(runTimes);
                await client.BatchCreateAppRunTimeAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to batch create app run times");
                return false;
            }
        }

        public async Task<ListAppRunTimesResponse?> ListAppRunTimesAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListAppRunTimesRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListAppRunTimesAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list app run times");
                return null;
            }
        }

        #endregion

        #region Store App Binary Operations

        public async Task<ListStoreAppBinariesResponse?> ListStoreAppBinariesAsync(TuiHub.Protos.Librarian.V1.InternalID appId, int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListStoreAppBinariesRequest
                {
                    AppId = appId,
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListStoreAppBinariesAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list store app binaries");
                return null;
            }
        }

        public async Task<DownloadStoreAppBinaryResponse?> DownloadStoreAppBinaryAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new DownloadStoreAppBinaryRequest { Id = id };
                return await client.DownloadStoreAppBinaryAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download store app binary");
                return null;
            }
        }

        #endregion
    }
}
