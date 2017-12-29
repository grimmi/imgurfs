using DownloaderHost.Toasts;
using ImgurFS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        public ObservableCollection<AlbumDownloadViewModel> Downloads { get; } = new ObservableCollection<AlbumDownloadViewModel>();

        private List<Task> runningTasks = new List<Task>();
        private Notifier notifier;

        public string DownloadPath { get; set; } = @"c:\temp\downloader";
        public AlbumDownloadViewModel SelectedDownload { get; set; }

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

        public void OpenFolder()
        {
            if(SelectedDownload == null)
            {
                return;
            }
            var path = Path.Combine(DownloadPath, SelectedDownload.AlbumName);
            Process.Start(path);
        }

        public void ReactToClipboardChange()
        {
            var clipboardText = Clipboard.GetText();
            if (!Downloads.Any(dl => clipboardText.Contains(dl.AlbumHash)) && IsImgurAlbumUrl(clipboardText))
            {
                var albumHash = clipboardText.Split('/').Last();
                var downloadTask = Task.Run(() =>
                {
                    var result = AlbumDownloader.DownloadAlbum(albumHash, DownloadPath);
                    return (albumName: result.Item1, imageCount: result.Item2);
                });
                var model = new AlbumDownloadViewModel(albumHash, downloadTask, NotifyCompletion);
                Downloads.Add(model);
                ShowToast(clipboardText);
            }
        }

        private void ShowToast(string dlUrl)
        {
            notifier.ShowDownloadNotification(dlUrl);
        }

        private void NotifyCompletion(AlbumDownloadViewModel albumDownload)
        {
            notifier.ShowDownloadedNotification(albumDownload.AlbumName, albumDownload.ImageCount);
        }

        private bool IsImgurAlbumUrl(string candidate)
        {
            return (candidate.StartsWith("http") || candidate.StartsWith("https"))
                && candidate.Contains("imgur.com") &&
                (candidate.Contains("/a/") || candidate.Contains("/gallery/"));
        }
    }
}
