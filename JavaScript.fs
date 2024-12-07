module MyBoleroApp.JavaScript

open Microsoft.JSInterop

let mutable runtime : IJSRuntime = null

let createUrl (stream: System.IO.Stream) =
    async {
        use streamRef = new DotNetStreamReference(stream)
        return! runtime.InvokeAsync<string>("makeUrl", streamRef).AsTask() |> Async.AwaitTask
    }

let revokeUrl (url: string) =
    runtime.InvokeVoidAsync("revokeUrl", url).AsTask() |> Async.AwaitTask
