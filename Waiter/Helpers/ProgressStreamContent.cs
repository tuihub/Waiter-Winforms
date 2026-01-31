using System.Net;

namespace Waiter.Helpers
{
    /// <summary>
    /// HTTP content that wraps a stream and reports upload progress.
    /// </summary>
    public class ProgressStreamContent : HttpContent
    {
        private readonly Stream _stream;
        private readonly IProgress<int>? _progress;
        private readonly long _totalLength;
        private const int BufferSize = 81920; // 80 KB buffer

        public ProgressStreamContent(Stream stream, IProgress<int>? progress, long totalLength)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _progress = progress;
            _totalLength = totalLength;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            var buffer = new byte[BufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            int lastReportedProgress = -1;

            while ((bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await stream.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;

                if (_progress != null && _totalLength > 0)
                {
                    var currentProgress = (int)(totalBytesRead * 100 / _totalLength);
                    if (currentProgress != lastReportedProgress)
                    {
                        _progress.Report(currentProgress);
                        lastReportedProgress = currentProgress;
                    }
                }
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _totalLength;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
