namespace ImgurFS

open FSharp.Data
open System
open System.IO

module ImageDownloader = 
      
    let clientID = File.ReadAllLines @"c:\temp\tdd.txt" |> Seq.head |> String.filter(fun c -> not(c.Equals('#')))

    let DownloadImageFromHash targetFolder imageHash =
        let image = sprintf "https://api.imgur.com/3/image/%s" imageHash
                    |> Http.RequestStream
        
        let targetPath = Path.Combine(targetFolder, imageHash + ".jpg")

        