namespace ImgurFS

open FSharp.Data
open System
open System.IO

module AlbumDownloader =

let clientID = File.ReadAllLines @"c:\temp\tdd.txt" |> Seq.head |> String.filter(fun c -> not(c.Equals('#')))
let invalidChars = Array.concat [|Path.GetInvalidFileNameChars();Path.GetInvalidPathChars()|]

let SaveStreamFromHttpResponse (filePath:string) (response:HttpResponseWithStream) =
    use saveStream = File.Create(filePath)
    response.ResponseStream.CopyTo(saveStream)

let CleanAlbumName = String.filter(fun c -> not(invalidChars |> Seq.exists(fun x -> x = c)))
                
let GetImageLinksOfAlbum (album:JsonValue) =
    (album, (album.GetProperty "images").AsArray() |> Array.map(fun img -> (img.GetProperty "link").AsString()))

let GetAlbumNameOrId (album:JsonValue) =
    match (album.GetProperty "title").AsString() |> CleanAlbumName with
    |"" | null -> (album.GetProperty "id").AsString()
    |name -> name

let MakeDownloadPath targetPath (album : JsonValue) =
    let downloadPath = Path.Combine(targetPath, (GetAlbumNameOrId album))
    if not (Directory.Exists downloadPath) then (Directory.CreateDirectory(downloadPath) |> ignore)
    downloadPath

let MakeImageName (link:string) (index:int) =
    sprintf "%03i - %s" index (Array.last (link.Split('/')))

let DownloadImage link downloadPath =
    let response = Http.RequestStream(link, httpMethod = "GET")
    SaveStreamFromHttpResponse downloadPath response

let DownloadImages targetPath (albumLinks : JsonValue * string[]) =
    let album, links = albumLinks
    let downloadFolder = MakeDownloadPath targetPath album
    links |> Seq.iteri(fun idx link -> let imagePath = Path.Combine(downloadFolder, (MakeImageName link idx))
                                       DownloadImage link imagePath
                                       printfn "[%d / %d] %s" (idx + 1) links.Length imagePath)

let DownloadAlbumData url =
    (Http.RequestString(url, httpMethod = "GET", headers = [ "Authorization", sprintf "Client-ID %s" clientID ])
     |> JsonValue.Parse).GetProperty "data"

let DownloadAlbum albumHash targetFolder=
    sprintf "https://api.imgur.com/3/album/%s" albumHash 
    |> DownloadAlbumData
    |> GetImageLinksOfAlbum
    |> DownloadImages targetFolder
        
let DownloadAlbumsFromFile targetFolder sourceFile =
    let albumHashes = File.ReadAllLines sourceFile 
                      |> Array.map(fun line -> line.Trim()) 
                      |> Array.filter(fun line -> not(line.StartsWith "#" || Seq.isEmpty line))
                      |> Array.map(fun (line:string) -> Array.last(line.Split('/')))
    albumHashes |> Array.iteri(fun idx hash -> printfn "[%d / %d] processing %s..." (idx + 1) albumHashes.Length hash
                                               DownloadAlbum hash targetFolder)
