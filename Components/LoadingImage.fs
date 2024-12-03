module MyBoleroApp.Components.LoadingImage

open System.Net.Http
open Microsoft.JSInterop
open Elmish
open Bolero
open Bolero.Html
open MudBlazor
open MyBoleroApp.Common

type State =
    | Loading
    | Done of Result<string * Dispose, string>

type Model =
    { Url: string
      State: State }
    member this.IsLoading = this.State.IsLoading

type Msg =
    | StartLoad
    | EndLoad of Result<string * Dispose, string>

let private loadCmd (js: IJSRuntime, http: HttpClient) (imageUrl: string) =
    Cmd.OfTask.either
        (fun imageUrl -> task {
            use! stream =
                if random.Next() % 2 = 0 then
                    http.GetStreamAsync(imageUrl: string)
                else
                    failwith "Something went wrong"
            printfn "%d KB" (stream.Length/1024L)
            use streamRef = new DotNetStreamReference(stream)
            return! js.InvokeAsync<string>("makeUrl", streamRef)
        })
        imageUrl
        (fun blobUrl ->
            let dispose () =
                js.InvokeVoidAsync("revokeUrl", blobUrl).AsTask() |> Async.AwaitTask |> Async.StartImmediate
            EndLoad (Ok (blobUrl, dispose))
        )
        (fun ex -> EndLoad (Error ex.Message))

let init (js, http) url =
    { Url = url
      State = Loading },
    loadCmd (js, http) url

let private disposeResult = function
    | Ok (_, dispose) -> dispose()
    | _ -> ()

let update (js, http) msg model =
    match msg, model.State with
    | StartLoad, Loading ->
        model, Cmd.none
    | StartLoad, Done result ->
        disposeResult result
        { model with State = Loading }, loadCmd (js, http) model.Url

    | EndLoad result, Loading ->
        { model with State = Done result }, Cmd.none
    | EndLoad _, Done result ->
        disposeResult result
        model, Cmd.none

let dispose model =
    match model.State with
    | Done result -> disposeResult result
    | _ -> ()

let clean msg =
    match msg with
    | EndLoad result -> disposeResult result
    | _ -> ()

let render model =
    match model.State with
    | Loading ->
        comp<MudProgressLinear> {
            attr.Indeterminate true
        }
    | Done (Ok (blobUrl, _)) ->
        comp<MudImage> {
            attr.Src blobUrl
            attr.ObjectFit ObjectFit.Cover
            attr.Class "rounded-lg"
        }
    | Done (Error text) ->
        comp<MudText> {
            attr.Color Color.Error
            attr.style "font-family:monospace"
            text
        }
