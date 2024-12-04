module MyBoleroApp.JS

open Microsoft.JSInterop

let mutable runtime : IJSRuntime = null

let makeUrl (stream: System.IO.Stream) =
    async {
        use streamRef = new DotNetStreamReference(stream)
        return! runtime.InvokeAsync<string>("makeUrl", streamRef).AsTask() |> Async.AwaitTask
    }

let revokeUrl (url: string) =
    #if DEBUG
    printfn "JS.revokeUrl: %s" url
    #endif
    runtime.InvokeVoidAsync("revokeUrl", url).AsTask() |> Async.AwaitTask
