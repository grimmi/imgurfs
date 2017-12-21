using ImgurFS;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DownloaderHost
{
    public class MainWindowViewModel
    {
        public ObservableCollection<string> DownloadUrls { get; } = new ObservableCollection<string>();

        private List<Task> runningTasks = new List<Task>();

        public MainWindowViewModel()
        {
            ClipboardMonitor.ClipboardContentChanged += (s, e) => ReactToClipboardChange();
        }

        public void ReactToClipboardChange()
        {
            var clipboardText = Clipboard.GetText();
            if(!DownloadUrls.Contains(clipboardText) && IsImgurAlbumUrl(clipboardText))
            {
                DownloadUrls.Add(clipboardText);
                var albumHash = clipboardText.Split('/').Last();
                var albumTask = Task.Run(() => { AlbumDownloader.DownloadAlbum(albumHash, @"c:\temp\cbdownload"); return albumHash; });
                albumTask.ContinueWith(NotifyCompletion);
            }
        }

        private void NotifyCompletion(Task<string> t)
        {
            if(t.IsCompleted)
            {
                var album = t.Result;
                MessageBox.Show($"album {album} finished downloading");
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
