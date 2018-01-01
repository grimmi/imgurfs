namespace DownloaderHost
{
    public abstract class DownloadViewModel
    {
        public abstract string DownloadHash { get; }
        public abstract string Title { get; }
        public abstract string GetPath();
    }
}
