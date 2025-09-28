module MyBoleroApp.Js

open System
open System.IO
open Microsoft.JSInterop

let revokeUrl (jsRuntime: IJSRuntime) (url: string) =
    jsRuntime.InvokeVoidAsync("revokeUrl", url).AsTask() |> Async.AwaitTask

type URL (jsRuntime: IJSRuntime, value: string) =
    member this.Value = value

    interface IDisposable with
        member this.Dispose() = revokeUrl jsRuntime value |> Async.StartImmediate

let createUrl (jsRuntime: IJSRuntime) (stream: Stream) (contentType: string option) =
    async {
        use stream = new DotNetStreamReference(stream)
        let contentType = contentType |> Option.toObj
        let! url = jsRuntime.InvokeAsync<string>("makeUrl", stream, contentType).AsTask() |> Async.AwaitTask
        return new URL(jsRuntime, url)
    }

module LocalStorage =
    let set (jsRuntime: IJSRuntime) (key: string) (value: 'a) =
        async {
            do! jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value).AsTask() |> Async.AwaitTask
        }

    let get (jsRuntime: IJSRuntime) (key: string) =
        async {
            return! jsRuntime.InvokeAsync<string>("localStorage.getItem", key).AsTask() |> Async.AwaitTask
        }

    let remove (jsRuntime: IJSRuntime) (key: string) =
        async {
            do! jsRuntime.InvokeVoidAsync("localStorage.removeItem", key).AsTask() |> Async.AwaitTask
        }
