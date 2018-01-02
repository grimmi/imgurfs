using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DownloaderHost
{
    //see https://stackoverflow.com/a/11901709/1344058
    public class ClipboardMonitor
    {
        public static event EventHandler ClipboardContentChanged;

        private static MonitorForm monitorForm = new MonitorForm();

        private static void OnClipboardContentChanged(EventArgs e)
        {
            ClipboardContentChanged?.Invoke(null, e);
        }

        private class MonitorForm : Form
        {
            public MonitorForm()
            {
                Win32Methods.SetParent(Handle, Win32Methods.HWND_MESSAGE);
                Win32Methods.AddClipboardFormatListener(Handle);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == Win32Methods.WM_CLIPBOARDUPDATE)
                {
                    OnClipboardContentChanged(null);
                }
                base.WndProc(ref m);
            }
        }

        public static void Shutdown()
        {
            monitorForm.Dispose();
            monitorForm = null;
        }
    }

    internal static class Win32Methods
    {
        public const int WM_CLIPBOARDUPDATE = 0x031D;
        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }
}
