module MyBoleroApp.Components.Image

open System
open Elmish
open Bolero
open Bolero.Html
open MudBlazor
open MyBoleroApp

type Data =
    { BlobUrl: Js.URL
      SizeInBytes: int64
      LoadingTime: TimeSpan }

type State =
    | Loading
    | Done of Result<Data, string>

type Model =
    { Url: string
      State: State }
    member this.IsLoading =
        this.State.IsLoading

    member this.Data =
        match this.State with
        | Done (Ok data) -> Some data
        | _ -> None

type Msg =
    | StartLoad of url: string
    | EndLoad of Result<Data, string>

let private loadCmd (imageUrl: string) =
    Cmd.OfAsync.either
        (fun imageUrl -> async {
            let start = DateTime.Now
            use! stream = Http.getStream imageUrl
            let length = stream.Length
            let! blobUrl = Js.createUrl stream
            return { BlobUrl = blobUrl
                     SizeInBytes = length
                     LoadingTime = DateTime.Now - start }
        })
        imageUrl
        (fun data -> EndLoad (Ok data))
        (fun ex -> EndLoad (Error ex.Message))

let init url =
    { Url = url
      State = Loading },
    loadCmd url

let private dispose = function
    | Ok data -> data.BlobUrl.Dispose()
    | _ -> ()

let update msg model =
    match msg, model.State with
    | StartLoad _, Loading ->
        model, Cmd.none

    | StartLoad url, Done result ->
        dispose result
        { model with Url = url; State = Loading },
        loadCmd url

    | EndLoad result, Loading ->
        { model with State = Done result }, Cmd.none

    | EndLoad result, _ ->
        dispose result
        model, Cmd.none

let render model =
    match model.State with
    | Loading ->
        comp<MudProgressLinear> {
            attr.Indeterminate true
            attr.Color Color.Primary
            attr.Size Size.Medium
            attr.Rounded true
        }
    | Done (Ok data) ->
        comp<MudImage> {
            attr.Src data.BlobUrl.Value
            attr.ObjectFit ObjectFit.Cover
            attr.Class "rounded"
        }
    | Done (Error str) ->
        comp<MudText> {
            attr.Color Color.Error
            attr.style "font-family: monospace; overflow-wrap: break-word"
            str
        }
