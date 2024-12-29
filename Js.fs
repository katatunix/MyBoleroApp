module MyBoleroApp.Js

open System
open System.IO
open Microsoft.JSInterop

let revokeUrl (js: IJSRuntime) (url: string) =
#if DEBUG
    printfn "Js.revokeUrl: %s" url
#endif
    js.InvokeVoidAsync("revokeUrl", url).AsTask() |> Async.AwaitTask

type URL (js: IJSRuntime, value: string) =
    member this.Value = value

    interface IDisposable with
        member this.Dispose() = revokeUrl js value |> Async.StartImmediate

let createUrl (js: IJSRuntime) (stream: Stream) (contentType: string option) =
    async {
        use stream = new DotNetStreamReference(stream)
        let contentType = contentType |> Option.toObj
        let! url = js.InvokeAsync<string>("makeUrl", stream, contentType).AsTask() |> Async.AwaitTask
#if DEBUG
        printfn "Js.createUrl: %s" url
#endif
        return new URL(js, url)
    }

module LocalStorage =
    let set (js: IJSRuntime) (key: string) (value: 'a) =
        js.InvokeVoidAsync("localStorage.setItem", key, value).AsTask() |> Async.AwaitTask

    let get (js: IJSRuntime) (key: string) =
        js.InvokeAsync<string>("localStorage.getItem", key).AsTask() |> Async.AwaitTask

    let remove (js: IJSRuntime) (key: string) =
        js.InvokeVoidAsync("localStorage.removeItem", key).AsTask() |> Async.AwaitTask
