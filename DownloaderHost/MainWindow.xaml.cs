using System.Windows;

namespace DownloaderHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClipboardMonitor monitor;

        public MainWindow()
        {
            InitializeComponent();
            monitor = new ClipboardMonitor();
        }
    }
}
