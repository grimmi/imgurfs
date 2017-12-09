namespace ImgurFS

module AlbumDownloader =
open FSharp.Data
open System

let clientID = ""

let downloadAlbum albumHash =
    let url = sprintf "https://api.imgur.com/3/album/%s/images" albumHash
    
    let response = Http.RequestString(url, httpMethod = "GET", headers = [ "Authorization", sprintf "Client ID %s" clientID ])

    printfn "%s" response


