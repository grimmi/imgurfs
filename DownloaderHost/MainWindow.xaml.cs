using System;
using System.ComponentModel;
using System.Windows;

namespace DownloaderHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();

            var notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("./Resources/album.ico");
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += (s, e) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if(WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ClipboardMonitor.Shutdown();
            base.OnClosing(e);
        }

        private void OpenFolderClicked(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).OpenFolder();
        }
    }
}
