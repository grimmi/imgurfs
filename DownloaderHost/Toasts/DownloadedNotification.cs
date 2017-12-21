using System.ComponentModel;
using ToastNotifications.Core;

namespace DownloaderHost.Toasts
{
    public class DownloadedNotification : NotificationBase, INotifyPropertyChanged
    {
        private NotificationDisplayPart displayPart;
        public override NotificationDisplayPart DisplayPart => displayPart ?? (displayPart = new DownloadedNotificationDisplayPart(this));

        public event PropertyChangedEventHandler PropertyChanged;

        private string message;
        public string Album
        {
            get { return message; }
            set { message = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Album))); }
        }

        private int imageCount;
        public int ImageCount
        {
            get { return imageCount; }
            set { imageCount = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageCount))); }
        }

        public DownloadedNotification(string albumName, int imageCount)
        {
            Album = albumName;
            ImageCount = imageCount;
        }
    }
}
