using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DownloaderHost
{
    public class ImageDownloadViewModel : DownloadViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _imageHash;
        public string ImageHash
        {
            get { return _imageHash; }
            set
            {
                _imageHash = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageHash)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        private string _imageName;
        public string ImageName
        {
            get { return _imageName; }
            set
            {
                _imageName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageName)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        public override string DownloadHash => ImageHash;

        public override string Title => string.IsNullOrWhiteSpace(ImageName) ? ImageHash : ImageName;

        private Task<string> _downloadTask;
        
        public ImageDownloadViewModel(string imageHash, Task<string> downloadTask, Action<ImageDownloadViewModel> onComplete = null)
        {
            ImageHash = imageHash;
            Downloading = true;
            _downloadTask = downloadTask;
            _downloadTask.ContinueWith(dlTask => { ImageName = dlTask.Result; Downloading = false; onComplete?.Invoke(this); });
        }

        public override string GetPath()
        {
            return ImageName;
        }
    }
}
