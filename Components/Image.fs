module MyBoleroApp.Components.Image

open System
open Elmish
open Bolero
open Bolero.Html
open MudBlazor
open MyBoleroApp

type Data =
    { blobUrl: Js.URL
      sizeInBytes: int64
      loadingTime: TimeSpan }

type State =
    | Loading
    | Done of Result<Data, string>

type Model =
    { url: string
      state: State }
    member this.IsLoading =
        this.state.IsLoading

    member this.Data =
        match this.state with
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
            return { blobUrl = blobUrl
                     sizeInBytes = length
                     loadingTime = DateTime.Now - start }
        })
        imageUrl
        (fun data -> EndLoad (Ok data))
        (fun ex -> EndLoad (Error ex.Message))

let init url =
    { url = url
      state = Loading },
    loadCmd url

let private dispose = function
    | Ok data -> data.blobUrl.Dispose()
    | _ -> ()

let update msg model =
    match msg, model.state with
    | StartLoad _, Loading ->
        model, Cmd.none

    | StartLoad url, Done result ->
        dispose result
        { model with url = url; state = Loading },
        loadCmd url

    | EndLoad result, Loading ->
        { model with state = Done result }, Cmd.none

    | EndLoad result, _ ->
        dispose result
        model, Cmd.none

let render (extraStyle: string option) (model: Model) =
    match model.state with
    | Loading ->
        comp<MudProgressLinear> {
            attr.Indeterminate true
            attr.Color Color.Primary
            attr.Size Size.Medium
            attr.Rounded true
        }
    | Done (Ok data) ->
        comp<MudImage> {
            attr.Src data.blobUrl.Value
            attr.ObjectFit ObjectFit.Cover
            attr.Class "rounded"
            match extraStyle with
            | Some style ->
                attr.style style
            | None ->
                attr.empty()
        }
    | Done (Error msg) ->
        comp<MudText> {
            attr.Color Color.Error
            attr.Typo Typo.body1
            attr.style "font-family: monospace;
                        overflow-wrap: break-word;"
            msg
        }