#r @"F:\Git Repos\imgurfs\packages\FSharp.Data.2.4.3\lib\net45\FSharp.Data.dll"
open FSharp.Data
open System
open System.IO

let clientID = "c053e70b0e9d647"

let saveStream (targetPath:string) (fileName:string) (index:int) (response:HttpResponseWithStream) =
    let name = sprintf "%03i - %s" index (fileName.Split('/').[3])
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
    |>Seq.iter(fun (image, stream) -> printf "[%d / %d]" counter imageLength
                                      saveStream targetPath image counter stream
                                      counter <- counter + 1)

let sanitizeAlbumName name =
    let invalidChars = Array.concat [|Path.GetInvalidFileNameChars();Path.GetInvalidPathChars()|]

    name |> String.filter(fun c -> not(invalidChars |> Seq.exists(fun x -> x = c)))
                
let inline last (arr:_[]) = arr.[arr.Length - 1]

let downloadAlbum albumHash =
    let url = sprintf "https://api.imgur.com/3/album/%s" albumHash
    
    let albumString = Http.RequestString(url, httpMethod = "GET", headers = [ "Authorization", sprintf "Client-ID %s" clientID ])
    
    let albumJson = JsonValue.Parse albumString

    let data = albumJson.GetProperty "data"

    let albumName = (data.GetProperty "title").AsString() |> sanitizeAlbumName
    
    let dataElements = (data.GetProperty "images").AsArray()

    let links = dataElements |> Array.map(fun d -> (d.GetProperty "link").AsString())

    links |> downloadImages (sprintf @"c:\temp\fsdownloadr\%s" albumName)
        
let albumLines = File.ReadAllLines @"c:\temp\tdd.txt" 
albumLines 
|> Array.filter(fun line -> not(System.String.IsNullOrWhiteSpace line))
|> Array.map(fun (line:string) -> Array.last(line.Split('/')))
|> Array.indexed 
|> Array.iter(fun (idx, album) -> printfn "downloading %s... (%d / %d)" album (idx + 1) albumLines.Length 
                                  downloadAlbum album)