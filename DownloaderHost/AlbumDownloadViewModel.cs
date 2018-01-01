using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DownloaderHost
{
    public class AlbumDownloadViewModel : DownloadViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override string DownloadHash => AlbumHash;

        private string _albumHash;
        public string AlbumHash
        {
            get { return _albumHash; }
            set
            {
                _albumHash = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlbumHash)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        private string _albumUrl;
        public string AlbumUrl
        {
            get { return _albumUrl; }
            set
            {
                _albumUrl = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlbumUrl)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }
        private string _albumName;
        public string AlbumName
        {
            get { return _albumName; }
            private set
            {
                _albumName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlbumName)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        private int _imageCount;
        public int ImageCount
        {
            get { return _imageCount; }
            set
            {
                _imageCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageCountDisplay)));
            }
        }

        private bool _downloading;
        public bool Downloading
        {
            get { return _downloading; }
            set
            {
                _downloading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Downloading)));
            }
        }

        public string ImageCountDisplay
        {
            get
            {
                return ImageCount == 0 ? "??" : ImageCount.ToString();
            }
        }

        public override string Title
        {
            get
            {
                return $"{AlbumName ?? AlbumHash} ({ImageCountDisplay} images)";
            }
        }

        private Task<(string albumName, int imageCount)> _downloadTask;
        private Action<AlbumDownloadViewModel> _onComplete;

        public AlbumDownloadViewModel(string albumHash, Task<(string albumName, int imageCount)> downloadTask, Action<AlbumDownloadViewModel> onComplete = null)
        {
            _albumHash = albumHash;
            _downloadTask = downloadTask;
            _downloadTask.ContinueWith(result => UpdateModel(result));
            Downloading = true;
            _onComplete = onComplete;
        }

        private void UpdateModel(Task<(string albumName, int imageCount)> finishedTask)
        {
            var result = finishedTask.Result;
            AlbumName = result.albumName;
            ImageCount = result.imageCount;
            Downloading = false;
            _onComplete?.Invoke(this);
        }

        public override string GetPath()
        {
            return AlbumName;
        }
    }
}
