module MyBoleroApp.Http

open System
open System.IO
open System.Net.Http

type StreamResponse(response: IDisposable, stream: Stream, contentType: string option) =
    member this.Stream = stream
    member this.ContentType = contentType

    interface IDisposable with
        member this.Dispose() =
            stream.Dispose()
            response.Dispose()

let getStream (client: HttpClient) (url: string) =
    async {
        let! response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead) |> Async.AwaitTask

        response.EnsureSuccessStatusCode() |> ignore

        let contentType =
            Some response.Content.Headers.ContentType.MediaType

        let! stream = response.Content.ReadAsStreamAsync() |> Async.AwaitTask

        return new StreamResponse(response, stream, contentType)
    }
