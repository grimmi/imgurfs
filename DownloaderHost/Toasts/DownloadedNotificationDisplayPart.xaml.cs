using ToastNotifications.Core;

namespace DownloaderHost.Toasts
{
    /// <summary>
    /// Interaction logic for DownloadedNotificationDisplayPart.xaml
    /// </summary>
    public partial class DownloadedNotificationDisplayPart : NotificationDisplayPart
    {
        private DownloadedNotification notification;

        public DownloadedNotificationDisplayPart(DownloadedNotification notification)
        {
            this.notification = notification;
            DataContext = notification;
            InitializeComponent();
        }
    }
}
