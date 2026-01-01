using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1;
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
            await _tokenService.SetTokensWithUsernameAsync(username, response.AccessToken, response.RefreshToken);
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

        public async Task<bool> DeleteAppAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new DeleteAppRequest { Id = id };
                await client.DeleteAppAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete app");
                return false;
            }
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

        #region Gebura - App Save File Pin/Unpin and Capacity

        public async Task<bool> PinAppSaveFileAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new PinAppSaveFileRequest { Id = id };
                await client.PinAppSaveFileAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to pin app save file");
                return false;
            }
        }

        public async Task<bool> UnpinAppSaveFileAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UnpinAppSaveFileRequest { Id = id };
                await client.UnpinAppSaveFileAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unpin app save file");
                return false;
            }
        }

        public async Task<GetAppSaveFileCapacityResponse?> GetAppSaveFileCapacityAsync()
        {
            try
            {
                var client = GetAuthenticatedClient();
                return await client.GetAppSaveFileCapacityAsync(new GetAppSaveFileCapacityRequest());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get app save file capacity");
                return null;
            }
        }

        public async Task<bool> SetAppSaveFileCapacityAsync(AppSaveFileCapacityStrategy strategy)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new SetAppSaveFileCapacityRequest { Strategy = strategy };
                await client.SetAppSaveFileCapacityAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set app save file capacity");
                return false;
            }
        }

        public async Task<bool> DeleteAppRunTimeAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new DeleteAppRunTimeRequest { Id = id };
                await client.DeleteAppRunTimeAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete app run time");
                return false;
            }
        }

        public async Task<SumAppRunTimeResponse?> SumAppRunTimeAsync(IEnumerable<TuiHub.Protos.Librarian.V1.InternalID>? appIdFilter = null)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new SumAppRunTimeRequest();
                if (appIdFilter != null)
                {
                    request.AppIdFilter.AddRange(appIdFilter);
                }
                return await client.SumAppRunTimeAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sum app run time");
                return null;
            }
        }

        #endregion

        #region Store App Binary Files

        public async Task<ListStoreAppBinaryFilesResponse?> ListStoreAppBinaryFilesAsync(TuiHub.Protos.Librarian.V1.InternalID binaryId, int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListStoreAppBinaryFilesRequest
                {
                    AppBinaryId = binaryId,
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListStoreAppBinaryFilesAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list store app binary files");
                return null;
            }
        }

        public async Task<ListStoreAppSaveFilesResponse?> ListStoreAppSaveFilesAsync(TuiHub.Protos.Librarian.V1.InternalID appId, int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListStoreAppSaveFilesRequest
                {
                    AppId = appId,
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListStoreAppSaveFilesAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list store app save files");
                return null;
            }
        }

        public async Task<DownloadStoreAppSaveFileResponse?> DownloadStoreAppSaveFileAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new DownloadStoreAppSaveFileRequest { Id = id };
                return await client.DownloadStoreAppSaveFileAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download store app save file");
                return null;
            }
        }

        #endregion

        #region Tiphereth - User Management

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdateUserRequest { User = user };
                await client.UpdateUserAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user");
                return false;
            }
        }

        public async Task<RegisterUserResponse?> RegisterUserAsync(string username, string password)
        {
            try
            {
                var client = GetAnonymousClient();
                var request = new RegisterUserRequest
                {
                    Username = username,
                    Password = password
                };
                return await client.RegisterUserAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register user");
                return null;
            }
        }

        public async Task<RegisterDeviceResponse?> RegisterDeviceAsync(Device device, string? clientLocalId = null)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new RegisterDeviceRequest { DeviceInfo = device };
                if (clientLocalId != null)
                {
                    request.ClientLocalId = clientLocalId;
                }
                return await client.RegisterDeviceAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register device");
                return null;
            }
        }

        public async Task<ListUserSessionsResponse?> ListUserSessionsAsync()
        {
            try
            {
                var client = GetAuthenticatedClient();
                return await client.ListUserSessionsAsync(new ListUserSessionsRequest());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list user sessions");
                return null;
            }
        }

        public async Task<bool> DeleteUserSessionAsync(TuiHub.Protos.Librarian.V1.InternalID sessionId)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new DeleteUserSessionRequest { SessionId = sessionId };
                await client.DeleteUserSessionAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user session");
                return false;
            }
        }

        #endregion

        #region Tiphereth - Account Linking

        public async Task<bool> LinkAccountAsync(TuiHub.Protos.Librarian.V1.FeatureRequest? config = null)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new LinkAccountRequest();
                if (config != null)
                {
                    request.Config = config;
                }
                await client.LinkAccountAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to link account");
                return false;
            }
        }

        public async Task<bool> UnLinkAccountAsync(TuiHub.Protos.Librarian.V1.InternalID accountId)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UnLinkAccountRequest { AccountId = accountId };
                await client.UnLinkAccountAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unlink account");
                return false;
            }
        }

        public async Task<ListLinkAccountsResponse?> ListLinkAccountsAsync(TuiHub.Protos.Librarian.V1.InternalID? userId = null)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListLinkAccountsRequest();
                if (userId != null)
                {
                    request.UserId = userId;
                }
                return await client.ListLinkAccountsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list link accounts");
                return null;
            }
        }

        #endregion

        #region Yesod - Feed Management

        public async Task<TuiHub.Protos.Librarian.V1.InternalID?> CreateFeedConfigAsync(FeedConfig config)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new CreateFeedConfigRequest { Config = config };
                var response = await client.CreateFeedConfigAsync(request);
                return response.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create feed config");
                return null;
            }
        }

        public async Task<bool> UpdateFeedConfigAsync(FeedConfig config)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdateFeedConfigRequest { Config = config };
                await client.UpdateFeedConfigAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update feed config");
                return false;
            }
        }

        public async Task<ListFeedConfigsResponse?> ListFeedConfigsAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListFeedConfigsRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListFeedConfigsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list feed configs");
                return null;
            }
        }

        public async Task<ListFeedItemsResponse?> ListFeedItemsAsync(IEnumerable<TuiHub.Protos.Librarian.V1.InternalID>? feedConfigIds = null, int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListFeedItemsRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                if (feedConfigIds != null)
                {
                    request.FeedIdFilter.AddRange(feedConfigIds);
                }
                return await client.ListFeedItemsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list feed items");
                return null;
            }
        }

        public async Task<GetFeedItemResponse?> GetFeedItemAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new GetFeedItemRequest { Id = id };
                return await client.GetFeedItemAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get feed item");
                return null;
            }
        }

        public async Task<bool> ReadFeedItemAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ReadFeedItemRequest { Id = id };
                await client.ReadFeedItemAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read feed item");
                return false;
            }
        }

        public async Task<GetBatchFeedItemsResponse?> GetBatchFeedItemsAsync(IEnumerable<TuiHub.Protos.Librarian.V1.InternalID> ids)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new GetBatchFeedItemsRequest();
                request.Ids.AddRange(ids);
                return await client.GetBatchFeedItemsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get batch feed items");
                return null;
            }
        }

        public async Task<ListFeedCategoriesResponse?> ListFeedCategoriesAsync()
        {
            try
            {
                var client = GetAuthenticatedClient();
                return await client.ListFeedCategoriesAsync(new ListFeedCategoriesRequest());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list feed categories");
                return null;
            }
        }

        public async Task<ListFeedPlatformsResponse?> ListFeedPlatformsAsync()
        {
            try
            {
                var client = GetAuthenticatedClient();
                return await client.ListFeedPlatformsAsync(new ListFeedPlatformsRequest());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list feed platforms");
                return null;
            }
        }

        #endregion

        #region Yesod - Feed Item Collections

        public async Task<bool> CreateFeedItemCollectionAsync(FeedItemCollection collection)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new CreateFeedItemCollectionRequest { Collection = collection };
                await client.CreateFeedItemCollectionAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create feed item collection");
                return false;
            }
        }

        public async Task<bool> UpdateFeedItemCollectionAsync(FeedItemCollection collection)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdateFeedItemCollectionRequest { Collection = collection };
                await client.UpdateFeedItemCollectionAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update feed item collection");
                return false;
            }
        }

        public async Task<ListFeedItemCollectionsResponse?> ListFeedItemCollectionsAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListFeedItemCollectionsRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListFeedItemCollectionsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list feed item collections");
                return null;
            }
        }

        public async Task<bool> AddFeedItemToCollectionAsync(TuiHub.Protos.Librarian.V1.InternalID collectionId, TuiHub.Protos.Librarian.V1.InternalID feedItemId)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new AddFeedItemToCollectionRequest
                {
                    CollectionId = collectionId,
                    FeedItemId = feedItemId
                };
                await client.AddFeedItemToCollectionAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add feed item to collection");
                return false;
            }
        }

        public async Task<bool> RemoveFeedItemFromCollectionAsync(TuiHub.Protos.Librarian.V1.InternalID collectionId, TuiHub.Protos.Librarian.V1.InternalID feedItemId)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new RemoveFeedItemFromCollectionRequest
                {
                    CollectionId = collectionId,
                    FeedItemId = feedItemId
                };
                await client.RemoveFeedItemFromCollectionAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove feed item from collection");
                return false;
            }
        }

        public async Task<ListFeedItemsInCollectionResponse?> ListFeedItemsInCollectionAsync(IEnumerable<TuiHub.Protos.Librarian.V1.InternalID>? collectionIdFilter = null, int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListFeedItemsInCollectionRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                if (collectionIdFilter != null)
                {
                    request.CollectionIdFilter.AddRange(collectionIdFilter);
                }
                return await client.ListFeedItemsInCollectionAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list feed items in collection");
                return null;
            }
        }

        #endregion

        #region Yesod - Feed Action Sets

        public async Task<TuiHub.Protos.Librarian.V1.InternalID?> CreateFeedActionSetAsync(FeedActionSet actionSet)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new CreateFeedActionSetRequest { ActionSet = actionSet };
                var response = await client.CreateFeedActionSetAsync(request);
                return response.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create feed action set");
                return null;
            }
        }

        public async Task<bool> UpdateFeedActionSetAsync(FeedActionSet actionSet)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdateFeedActionSetRequest { ActionSet = actionSet };
                await client.UpdateFeedActionSetAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update feed action set");
                return false;
            }
        }

        public async Task<ListFeedActionSetsResponse?> ListFeedActionSetsAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListFeedActionSetsRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListFeedActionSetsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list feed action sets");
                return null;
            }
        }

        #endregion

        #region Netzach - Notification Management

        public async Task<TuiHub.Protos.Librarian.V1.InternalID?> CreateNotifyTargetAsync(NotifyTarget target)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new CreateNotifyTargetRequest { Target = target };
                var response = await client.CreateNotifyTargetAsync(request);
                return response.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create notify target");
                return null;
            }
        }

        public async Task<bool> UpdateNotifyTargetAsync(NotifyTarget target)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdateNotifyTargetRequest { Target = target };
                await client.UpdateNotifyTargetAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update notify target");
                return false;
            }
        }

        public async Task<ListNotifyTargetsResponse?> ListNotifyTargetsAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListNotifyTargetsRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListNotifyTargetsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list notify targets");
                return null;
            }
        }

        public async Task<TuiHub.Protos.Librarian.V1.InternalID?> CreateNotifyFlowAsync(NotifyFlow flow)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new CreateNotifyFlowRequest { Flow = flow };
                var response = await client.CreateNotifyFlowAsync(request);
                return response.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create notify flow");
                return null;
            }
        }

        public async Task<bool> UpdateNotifyFlowAsync(NotifyFlow flow)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdateNotifyFlowRequest { Flow = flow };
                await client.UpdateNotifyFlowAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update notify flow");
                return false;
            }
        }

        public async Task<ListNotifyFlowsResponse?> ListNotifyFlowsAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListNotifyFlowsRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListNotifyFlowsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list notify flows");
                return null;
            }
        }

        #endregion

        #region Netzach - System Notifications

        public async Task<ListSystemNotificationsResponse?> ListSystemNotificationsAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListSystemNotificationsRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListSystemNotificationsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list system notifications");
                return null;
            }
        }

        public async Task<bool> UpdateSystemNotificationAsync(TuiHub.Protos.Librarian.V1.InternalID id, SystemNotificationStatus status)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdateSystemNotificationRequest
                {
                    Id = id,
                    Status = status
                };
                await client.UpdateSystemNotificationAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update system notification");
                return false;
            }
        }

        #endregion

        #region Chesed - Image Management

        public async Task<UploadImageResponse?> UploadImageAsync(TuiHub.Protos.Librarian.V1.FileMetadata fileMetadata)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UploadImageRequest { FileMetadata = fileMetadata };
                return await client.UploadImageAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload image");
                return null;
            }
        }

        public async Task<bool> UpdateImageAsync(TuiHub.Protos.Librarian.V1.InternalID id, string? name = null, string? description = null)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdateImageRequest { Id = id };
                if (name != null) request.Name = name;
                if (description != null) request.Description = description;
                await client.UpdateImageAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update image");
                return false;
            }
        }

        public async Task<ListImagesResponse?> ListImagesAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListImagesRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListImagesAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list images");
                return null;
            }
        }

        public async Task<SearchImagesResponse?> SearchImagesAsync(string keywords, int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new SearchImagesRequest
                {
                    Keywords = keywords,
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.SearchImagesAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search images");
                return null;
            }
        }

        public async Task<GetImageResponse?> GetImageAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new GetImageRequest { Id = id };
                return await client.GetImageAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get image");
                return null;
            }
        }

        public async Task<DownloadImageResponse?> DownloadImageAsync(TuiHub.Protos.Librarian.V1.InternalID id)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new DownloadImageRequest { Id = id };
                return await client.DownloadImageAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download image");
                return null;
            }
        }

        #endregion

        #region Binah - File Operations

        public async Task<PresignedUploadFileResponse?> PresignedUploadFileAsync()
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new PresignedUploadFileRequest();
                return await client.PresignedUploadFileAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get presigned upload URL");
                return null;
            }
        }

        public async Task<PresignedUploadFileStatusResponse?> PresignedUploadFileStatusAsync(FileTransferStatus status)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new PresignedUploadFileStatusRequest { Status = status };
                return await client.PresignedUploadFileStatusAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get presigned upload status");
                return null;
            }
        }

        public async Task<PresignedDownloadFileResponse?> PresignedDownloadFileAsync()
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new PresignedDownloadFileRequest();
                return await client.PresignedDownloadFileAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get presigned download URL");
                return null;
            }
        }

        #endregion

        #region Angela - Porter Management

        public async Task<ListPorterDigestsResponse?> ListPorterDigestsAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListPorterDigestsRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListPorterDigestsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list porter digests");
                return null;
            }
        }

        public async Task<TuiHub.Protos.Librarian.V1.InternalID?> CreatePorterContextAsync(PorterContext context)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new CreatePorterContextRequest { Context = context };
                var response = await client.CreatePorterContextAsync(request);
                return response.ContextId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create porter context");
                return null;
            }
        }

        public async Task<ListPorterContextsResponse?> ListPorterContextsAsync(int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new ListPorterContextsRequest
                {
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.ListPorterContextsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list porter contexts");
                return null;
            }
        }

        public async Task<bool> UpdatePorterContextAsync(PorterContext context)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new UpdatePorterContextRequest { Context = context };
                await client.UpdatePorterContextAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update porter context");
                return false;
            }
        }

        #endregion

        #region Gebura - App Info Search

        public async Task<SearchAppInfosResponse?> SearchAppInfosAsync(string nameLike, int pageSize = 10, int pageNum = 1)
        {
            try
            {
                var client = GetAuthenticatedClient();
                var request = new SearchAppInfosRequest
                {
                    NameLike = nameLike,
                    Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
                    {
                        PageSize = pageSize,
                        PageNum = pageNum
                    }
                };
                return await client.SearchAppInfosAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search app infos");
                return null;
            }
        }

        #endregion

        #region Binah - Storage Capacity

        public async Task<GetStorageCapacityUsageResponse?> GetStorageCapacityUsageAsync()
        {
            try
            {
                var client = GetAuthenticatedClient();
                return await client.GetStorageCapacityUsageAsync(new GetStorageCapacityUsageRequest());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get storage capacity usage");
                return null;
            }
        }

        #endregion
    }
}
