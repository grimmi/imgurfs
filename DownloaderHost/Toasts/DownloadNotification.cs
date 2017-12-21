using System.ComponentModel;
using ToastNotifications.Core;

namespace DownloaderHost.Toasts
{
    public class DownloadNotification : NotificationBase, INotifyPropertyChanged
    {
        private NotificationDisplayPart displayPart;
        public override NotificationDisplayPart DisplayPart => displayPart ?? (displayPart = new DownloadNotificationDisplayPart(this));

        public event PropertyChangedEventHandler PropertyChanged;

        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message))); }
        }

        public DownloadNotification(string url)
        {
            Message = $"now downloading {url}...";
        }
    }
}
