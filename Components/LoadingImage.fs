module MyBoleroApp.Components.LoadingImage

open System
open Elmish
open Bolero
open Bolero.Html
open MudBlazor
open MyBoleroApp
open MyBoleroApp.Common

type Data =
    { BlobUrl: string
      SizeInBytes: int64
      LoadingTime: TimeSpan }

type State =
    | Loading
    | Done of Result<Data, string>

type Model =
    { Url: string
      State: State }
    member this.IsLoading = this.State.IsLoading
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
            let! blobUrl = JS.makeUrl stream
            return { BlobUrl = blobUrl; SizeInBytes = length; LoadingTime = DateTime.Now - start }
        })
        imageUrl
        (Ok >> EndLoad)
        (exnMsg >> Error >> EndLoad)

let init url =
    { Url = url
      State = Loading },
    loadCmd url

let private revoke = function
    | Ok data -> JS.revokeUrl data.BlobUrl |> Async.StartImmediate
    | _ -> ()

let update msg model =
    match msg, model.State with
    | StartLoad _, Loading ->
        model, Cmd.none
    | StartLoad url, Done result ->
        revoke result
        { model with Url = url; State = Loading }, loadCmd url

    | EndLoad result, Loading ->
        { model with State = Done result }, Cmd.none
    | EndLoad _, Done result ->
        revoke result
        model, Cmd.none

let dispose model =
    match model.State with
    | Done result -> revoke result
    | _ -> ()

let clean msg =
    match msg with
    | EndLoad result -> revoke result
    | _ -> ()

let render model =
    match model.State with
    | Loading ->
        comp<MudProgressLinear> {
            attr.Indeterminate true
            attr.Color Color.Primary
        }
    | Done (Ok data) ->
        comp<MudImage> {
            attr.Src data.BlobUrl
            attr.ObjectFit ObjectFit.Cover
            attr.Class "rounded"
            on.load (fun _ -> JS.revokeUrl data.BlobUrl |> Async.StartImmediate)
        }
    | Done (Error text) ->
        comp<MudText> {
            attr.Color Color.Error
            attr.style "font-family: monospace"
            text
        }
