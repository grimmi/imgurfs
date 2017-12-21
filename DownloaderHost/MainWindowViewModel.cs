using DownloaderHost.Toasts;
using ImgurFS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace DownloaderHost
{
    public class MainWindowViewModel
    {
        public ObservableCollection<string> DownloadUrls { get; } = new ObservableCollection<string>();

        private List<Task> runningTasks = new List<Task>();
        private Notifier notifier;

        public MainWindowViewModel()
        {
            ClipboardMonitor.ClipboardContentChanged += (s, e) => ReactToClipboardChange();
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new PrimaryScreenPositionProvider(
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 75
                );
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(5),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(2));
                cfg.Dispatcher = Application.Current.Dispatcher;
                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 300;
            });
        }

        public void ReactToClipboardChange()
        {
            var clipboardText = Clipboard.GetText();
            if(!DownloadUrls.Contains(clipboardText) && IsImgurAlbumUrl(clipboardText))
            {
                DownloadUrls.Add(clipboardText);
                var albumHash = clipboardText.Split('/').Last();
                var albumTask = Task.Run(() => { var result = AlbumDownloader.DownloadAlbum(albumHash, @"c:\temp\cbdownload"); return (albumName: result.Item1, imageCount: result.Item2); });
                albumTask.ContinueWith(NotifyCompletion);
                ShowToast(clipboardText);
            }
        }

        private void ShowToast(string dlUrl)
        {
            notifier.ShowDownloadNotification(dlUrl);
        }

        private void NotifyCompletion(Task<(string albumName, int imageCount)> t)
        {
            if(t.IsCompleted)
            {
                var album = t.Result;
                notifier.ShowDownloadedNotification($"{album.albumName} ({album.imageCount} images)");
            }
        }

        private bool IsImgurAlbumUrl(string candidate)
        {
            return (candidate.StartsWith("http") || candidate.StartsWith("https"))
                && candidate.Contains("imgur.com") && 
                (candidate.Contains("/a/") || candidate.Contains("/gallery/"));

        }
    }
}
