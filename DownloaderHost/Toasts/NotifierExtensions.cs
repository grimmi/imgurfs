using ToastNotifications;

namespace DownloaderHost.Toasts
{
    public static class NotifierExtensions
    {
        public static void ShowDownloadNotification(this Notifier notifier, string message)
        {
            notifier.Notify<DownloadNotification>(() => new DownloadNotification(message));
        }
    }
}
