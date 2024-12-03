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

type Msg =
    | StartLoad
    | EndLoad of Result<string * Dispose, string>

let private loadCmd (js: IJSRuntime, http: HttpClient) (imageUrl: string) =
    Cmd.OfTask.either
        (fun imageUrl -> task {
            use! stream =
                http.GetStreamAsync(imageUrl: string)
                // failwith "Something went wrong"
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

let init (js: IJSRuntime, http: HttpClient) (url: string) =
    { Url = url
      State = Loading },
    loadCmd (js, http) url

let update (js: IJSRuntime, http: HttpClient) msg model =
    match msg, model.State with
    | StartLoad, Loading ->
        model, Cmd.none

    | StartLoad, Done result ->
        match result with
        | Ok (_, dispose) -> dispose()
        | _ -> ()
        { model with State = Loading }, loadCmd (js, http) model.Url

    | EndLoad result, Loading ->
        { model with State = Done result }, Cmd.none
    | EndLoad _, Done _ ->
        model, Cmd.none

let dispose model =
    match model.State with
    | Done (Ok (_, dispose)) -> dispose()
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
