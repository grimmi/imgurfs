#r @"F:\Git Repos\imgurfs\packages\FSharp.Data.2.4.3\lib\net45\FSharp.Data.dll"
open FSharp.Data
open System.IO

let clientID = File.ReadAllLines @"c:\temp\tdd.txt" |> Seq.head |> String.filter(fun c -> not(c.Equals('#')))
let invalidChars = Array.concat [|Path.GetInvalidFileNameChars();Path.GetInvalidPathChars()|]

let SaveStreamFromHttpResponse (filePath:string) (response:HttpResponseWithStream) =
    use saveStream = File.Create(filePath)
    response.ResponseStream.CopyTo(saveStream)

let CleanAlbumName name =
    name |> String.filter(fun c -> not(invalidChars |> Seq.exists(fun x -> x = c)))
                
let inline last (arr:_[]) = arr.[arr.Length - 1]

let GetImageLinksOfAlbum (album:JsonValue) =
    (album, (album.GetProperty "images").AsArray()
             |>Array.map(fun img -> (img.GetProperty "link").AsString()))

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

let DownloadImages targetPath (albumLinks : JsonValue * string[]) =
    let album, links = albumLinks
    let downloadFolder = MakeDownloadPath targetPath album
    let mutable counter = 1
    links |>Seq.iter(fun link -> let response = Http.RequestStream(link, httpMethod = "GET")
                                 let imagePath = Path.Combine(downloadFolder, (MakeImageName link counter))
                                 printfn "[%d / %d] %s" counter links.Length imagePath
                                 SaveStreamFromHttpResponse imagePath response
                                 counter <- counter + 1)
let DownloadAlbumData url =
    (Http.RequestString(url, httpMethod = "GET", headers = [ "Authorization", sprintf "Client-ID %s" clientID ])
     |>JsonValue.Parse).GetProperty "data"

let downloadAlbum albumHash targetFolder=
    sprintf "https://api.imgur.com/3/album/%s" albumHash 
    |> DownloadAlbumData
    |> GetImageLinksOfAlbum
    |> DownloadImages targetFolder
        
let albums = File.ReadAllLines @"c:\temp\tdd.txt"
             |> Array.filter(fun line -> not(line.StartsWith "#") && not(System.String.IsNullOrWhiteSpace line))
             |> Array.map(fun (line:string) -> Array.last(line.Split('/')))
             |> Array.indexed 
albums
|> Array.iter(fun (idx, album) -> printfn "downloading %s... (%d / %d)" album (idx + 1) albums.Length 
                                  downloadAlbum album @"c:\temp\fsdownloadr\")