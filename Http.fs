module MyBoleroApp.Http

open System
open System.IO
open System.Net.Http

type StreamResponse(response: IDisposable, stream: Stream, contentType: string option) =
    member _.Stream = stream
    member _.ContentType = contentType

    interface IDisposable with
        member _.Dispose() =
            stream.Dispose()
            response.Dispose()

let getStream (httpClient: HttpClient) (url: string) =
    async {
        let! response = httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead) |> Async.AwaitTask
        response.EnsureSuccessStatusCode() |> ignore
        let contentType = Some response.Content.Headers.ContentType.MediaType
        let! stream = response.Content.ReadAsStreamAsync() |> Async.AwaitTask
        return new StreamResponse(response, stream, contentType)
    }
