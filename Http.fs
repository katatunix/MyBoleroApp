module MyBoleroApp.Http

open System.Net.Http

let mutable client : HttpClient = null

let getStream (url: string) =
    client.GetStreamAsync(url) |> Async.AwaitTask
