module MyBoleroApp.Js

open System
open Microsoft.JSInterop

let mutable runtime : IJSRuntime = null

let revokeUrl (url: string) =
#if DEBUG
    printfn "Js.revokeUrl: %s" url
#endif
    runtime.InvokeVoidAsync("revokeUrl", url).AsTask() |> Async.AwaitTask

type URL (value: string) =
    member this.Value = value
    interface IDisposable with
        member this.Dispose() = revokeUrl value |> Async.StartImmediate
    member this.Dispose() =
        (this:IDisposable).Dispose()
    override this.ToString() =
        value

let createUrl (stream: System.IO.Stream) =
    async {
        use streamRef = new DotNetStreamReference(stream)
        let! url = runtime.InvokeAsync<string>("makeUrl", streamRef).AsTask() |> Async.AwaitTask
#if DEBUG
        printfn "Js.createUrl: %s" url
#endif
        return new URL(url)
    }

module LocalStorage =
    let set (key: string) value =
        runtime.InvokeVoidAsync("localStorage.setItem", key, value).AsTask() |> Async.AwaitTask

    let get (key: string) =
        runtime.InvokeAsync<string>("localStorage.getItem", key).AsTask() |> Async.AwaitTask

    let remove (key: string) =
        runtime.InvokeVoidAsync("localStorage.removeItem", key).AsTask() |> Async.AwaitTask
