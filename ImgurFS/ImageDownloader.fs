namespace ImgurFS

open FSharp.Data
open System
open System.IO

module ImageDownloader = 
      
    let clientID = File.ReadAllLines @"c:\temp\tdd.txt" |> Seq.head |> String.filter(fun c -> not(c.Equals('#')))

    let DownloadImageFromHash imageHash targetFolder =
        let url = sprintf "https://api.imgur.com/3/image/%s" imageHash
        let imageData = (Http.RequestString(url, httpMethod = "GET", headers = [ "Authorization", sprintf "Client-ID %s" clientID ])
                        |>JsonValue.Parse).GetProperty "data"

        let imageUrl = (imageData.GetProperty "link").AsString()

        let image = Http.RequestStream(imageUrl, httpMethod = "GET")

        let imageName = Array.last(imageUrl.Split('/'))
        let imagePath = Path.Combine(targetFolder, imageName)

        use imageStream = File.Create(imagePath)
        image.ResponseStream.CopyTo(imageStream)

        (imageHash, 1)

       
        