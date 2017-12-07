// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

//#load "Library1.fs"
//open ImgurFS
#r @"F:\Git Repos\imgurfs\packages\FSharp.Data.2.4.3\lib\net45\FSharp.Data.dll"
open FSharp.Data
open System
open System.IO

type Album = { images : seq<string> }

let clientID = "<yourclientid>"

let saveStream (targetPath:string) (fileName:string) (response:HttpResponseWithStream) =
    let name = fileName.Split('/').[3]
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
                                      counter <- counter + 1
                                      saveStream targetPath image stream)

let sanitizeAlbumName name =
    let invalidChars = Array.concat [|Path.GetInvalidFileNameChars();Path.GetInvalidPathChars()|]

    name |> String.filter(fun c -> not(invalidChars |> Seq.exists(fun x -> x = c)))
                

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
|> Array.indexed 
|> Array.iter(fun (idx, album) -> printfn "downloading %s... (%d / %d)" album (idx + 1) albumLines.Length 
                                  downloadAlbum album)