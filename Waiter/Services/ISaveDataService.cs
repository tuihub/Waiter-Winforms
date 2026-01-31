namespace Waiter.Services
{
    /// <summary>
    /// Interface for creating and managing compressed save data archives.
    /// </summary>
    public interface ISaveDataService
    {
        /// <summary>
        /// Creates a compressed ZIP archive of a save data directory.
        /// </summary>
        /// <param name="savePath">Directory containing save files</param>
        /// <param name="outputFileName">Output archive name (optional, auto-generated if null)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File info for created archive</returns>
        Task<FileInfo> CreateSaveArchiveAsync(
            string savePath,
            string? outputFileName = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Calculates SHA256 hash of a file.
        /// Used for file integrity verification.
        /// </summary>
        /// <param name="file">File to hash</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>SHA256 hash as lowercase hex string</returns>
        Task<string> CalculateSHA256Async(FileInfo file, CancellationToken cancellationToken);
    }
}
