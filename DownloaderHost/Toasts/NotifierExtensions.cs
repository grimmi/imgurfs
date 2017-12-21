using ToastNotifications;

namespace DownloaderHost.Toasts
{
    public static class NotifierExtensions
    {
        public static void ShowDownloadNotification(this Notifier notifier, string message)
        {
            notifier.Notify<DownloadNotification>(() => new DownloadNotification(message));
        }

        public static void ShowDownloadedNotification(this Notifier notifier, string album, int imageCount)
        {
            notifier.Notify<DownloadedNotification>(() => new DownloadedNotification(album, imageCount));
        }
    }
}
