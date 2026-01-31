using System.IO.Compression;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace Waiter.Services
{
    /// <summary>
    /// Service for creating and managing compressed save data archives.
    /// </summary>
    public class SaveDataService : ISaveDataService
    {
        private readonly ILogger<SaveDataService> _logger;

        public SaveDataService(ILogger<SaveDataService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<FileInfo> CreateSaveArchiveAsync(
            string savePath,
            string? outputFileName = null,
            CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(savePath))
            {
                throw new DirectoryNotFoundException($"Save data directory not found: {savePath}");
            }

            // Generate output filename if not provided
            outputFileName ??= $"save_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip";

            var tempPath = Path.Combine(Path.GetTempPath(), outputFileName);

            _logger.LogInformation("Creating save archive from '{SavePath}' to '{ArchivePath}'",
                savePath, tempPath);

            // Delete existing file if present
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            // Create archive - run on thread pool to not block
            await Task.Run(() =>
            {
                ZipFile.CreateFromDirectory(savePath, tempPath, CompressionLevel.Optimal, includeBaseDirectory: false);
            }, cancellationToken);

            var fileInfo = new FileInfo(tempPath);

            _logger.LogInformation("Save archive created: {Size} bytes", fileInfo.Length);

            return fileInfo;
        }

        /// <inheritdoc />
        public async Task<string> CalculateSHA256Async(FileInfo file, CancellationToken cancellationToken)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException($"File not found: {file.FullName}");
            }

            _logger.LogInformation("Calculating SHA256 for '{FilePath}' ({Size} bytes)",
                file.FullName, file.Length);

            using var sha256 = SHA256.Create();
            await using var stream = file.OpenRead();

            var hashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);
            var hashString = Convert.ToHexString(hashBytes).ToLowerInvariant();

            _logger.LogInformation("SHA256: {Hash}", hashString);

            return hashString;
        }
    }
}
