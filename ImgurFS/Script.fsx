open FSharp.Data
#r @"F:\Git Repos\imgurfs\packages\FSharp.Data.2.4.3\lib\net45\FSharp.Data.dll"
open FSharp.Data
open System
open System.IO

let clientID = File.ReadAllLines @"c:\temp\tdd.txt" |> Seq.head |> String.filter(fun c -> not(c.Equals('#')))

let saveStream (targetPath:string) (fileName:string) (index:int) (response:HttpResponseWithStream) =
    let name = sprintf "%03i - %s" index (Array.last (fileName.Split('/')))
    let targetFile = Path.Combine(targetPath, name)
    use saveStream = File.Create(targetFile)
    response.ResponseStream.CopyTo(saveStream)
    printfn "%s" targetFile

let downloadImages targetPath (images : seq<string>) =
    
    if not (Directory.Exists targetPath) then (Directory.CreateDirectory(targetPath) |> ignore)

    let imageLength = Seq.length images
    let mutable counter = 1

    images
    |>Seq.map(fun image -> (image, Http.RequestStream(image, httpMethod = "GET")))
    |>Seq.iter(fun (image, stream) -> printf "[%d / %d] " counter imageLength
                                      saveStream targetPath image counter stream
                                      counter <- counter + 1)

let sanitizeAlbumName name =
    let invalidChars = Array.concat [|Path.GetInvalidFileNameChars();Path.GetInvalidPathChars()|]

    name |> String.filter(fun c -> not(invalidChars |> Seq.exists(fun x -> x = c)))
                
let inline last (arr:_[]) = arr.[arr.Length - 1]

let downloadAlbum albumHash targetFolder=
    let url = sprintf "https://api.imgur.com/3/album/%s" albumHash
    
    let album = (Http.RequestString(url, httpMethod = "GET", headers = [ "Authorization", sprintf "Client-ID %s" clientID ])
                 |>JsonValue.Parse).GetProperty "data"

    let albumName = match (album.GetProperty "title").AsString() |> sanitizeAlbumName with
                    |"" | null -> albumHash
                    |name -> name
    
    (album.GetProperty "images").AsArray()
    |> Array.map(fun d -> (d.GetProperty "link").AsString())
    |> downloadImages (Path.Combine(targetFolder, albumName))
        
let albums = File.ReadAllLines @"c:\temp\tdd.txt"
             |> Array.filter(fun line -> not(line.StartsWith "#") && not(System.String.IsNullOrWhiteSpace line))
             |> Array.map(fun (line:string) -> Array.last(line.Split('/')))
             |> Array.indexed 
albums
|> Array.iter(fun (idx, album) -> printfn "downloading %s... (%d / %d)" album (idx + 1) albums.Length 
                                  downloadAlbum album @"c:\temp\fsdownloader\")