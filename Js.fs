module MyBoleroApp.Js

open Microsoft.JSInterop

let mutable runtime : IJSRuntime = null

let createUrl (stream: System.IO.Stream) =
    async {
        use streamRef = new DotNetStreamReference(stream)
        let! url = runtime.InvokeAsync<string>("makeUrl", streamRef).AsTask() |> Async.AwaitTask
        #if DEBUG
        printfn "createUrl: %s" url
        #endif
        return url
    }

let revokeUrl (url: string) =
    #if DEBUG
    printfn "revokeUrl: %s" url
    #endif
    runtime.InvokeVoidAsync("revokeUrl", url).AsTask() |> Async.AwaitTask
