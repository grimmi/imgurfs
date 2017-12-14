namespace ImgurFS


module ClipboardWatcher =
    open AlbumDownloader
    open System.Runtime.InteropServices
    open System.Windows.Forms
    open System

    type ClipboardMonitor() =
        inherit Control()
        
        [<DllImport("User32.dll")>]
        static extern int SetClipboardViewer(int hWndNewViewer)

        [<DllImport("User32.dll", CharSet = CharSet.Auto)>]
        static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext)

        [<DllImport("User32.dll", CharSet = CharSet.Auto)>]
        static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam)

        let mutable nextClipboardViewer : IntPtr = IntPtr.Zero
        
        let CheckForImgurUrl (url:string)=
            printfn "checking '%s'..." url
            if url.ToLower().StartsWith("http") && url.ToLower().Contains("imgur.com") then
                printfn "it's an imgur.com url!"
                url
            else 
                ""

        let ExtractAlbumHashFromUrl (url:string) =
            Array.last(url.Split('/'))

        let ReactToClipboard =
            match CheckForImgurUrl(Clipboard.GetText()) with
            |"" -> ()
            |url -> url |> ExtractAlbumHashFromUrl |> DownloadAlbum @"c:\temp\fsdownloadr"

        override this.WndProc(message : System.Windows.Forms.Message byref) =
            match message.Msg with
            |0x0308 -> ReactToClipboard
                       //SendMessage(nextClipboardViewer, message.Msg, message.WParam, message.LParam) |> ignore
            |_ -> ()

        //member public this.Init() =
           //nextClipboardViewer <- SetClipboardViewer(base.Handle |> int) |> IntPtr 
        