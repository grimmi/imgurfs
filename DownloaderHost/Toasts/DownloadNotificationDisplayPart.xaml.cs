using ToastNotifications.Core;

namespace DownloaderHost.Toasts
{
    /// <summary>
    /// Interaction logic for DownloadNotificationDisplayPart.xaml
    /// </summary>
    public partial class DownloadNotificationDisplayPart : NotificationDisplayPart
    {
        private DownloadNotification notification;

        public DownloadNotificationDisplayPart(DownloadNotification notification)
        {
            this.notification = notification;
            DataContext = notification;
            InitializeComponent();
        }
    }
}
