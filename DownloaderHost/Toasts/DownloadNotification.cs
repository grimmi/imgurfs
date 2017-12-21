using System.ComponentModel;
using ToastNotifications.Core;

namespace DownloaderHost.Toasts
{
    public class DownloadNotification : NotificationBase, INotifyPropertyChanged
    {
        private NotificationDisplayPart displayPart;
        public override NotificationDisplayPart DisplayPart => displayPart ?? (displayPart = new DownloadNotificationDisplayPart(this));

        public event PropertyChangedEventHandler PropertyChanged;

        private string albumUrl;
        public string AlbumUrl
        {
            get { return albumUrl; }
            set { albumUrl = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlbumUrl))); }
        }

        public DownloadNotification(string url)
        {
            AlbumUrl = url;
        }
    }
}
