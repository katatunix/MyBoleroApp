module MyBoleroApp.Js

open Microsoft.JSInterop

let mutable runtime : IJSRuntime = null

let createUrl (stream: System.IO.Stream) =
    async {
        use streamRef = new DotNetStreamReference(stream)
        let! url = runtime.InvokeAsync<string>("makeUrl", streamRef).AsTask() |> Async.AwaitTask
        printfn "createUrl: %s" url
        return url
    }

let revokeUrl (url: string) =
    printfn "revokeUrl: %s" url
    runtime.InvokeVoidAsync("revokeUrl", url).AsTask() |> Async.AwaitTask
