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
      Length: int64
      LoadingTime: TimeSpan }

type State =
    | Loading
    | Done of Result<Data, string>

type Model =
    { Url: string
      State: State }
    member this.IsLoading = this.State.IsLoading

type Msg =
    | StartLoad
    | EndLoad of Result<Data, string>

let private loadCmd (imageUrl: string) =
    Cmd.OfAsync.either
        (fun imageUrl -> async {
            let start = DateTime.Now
            use! stream = Http.getStream imageUrl
            let length = stream.Length
            let! blobUrl = JS.makeUrl stream
            return { BlobUrl = blobUrl; Length = length; LoadingTime = DateTime.Now - start }
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
    | StartLoad, Loading ->
        model, Cmd.none
    | StartLoad, Done result ->
        revoke result
        { model with State = Loading }, loadCmd model.Url

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
        }
    | Done (Ok data) ->
        comp<MudStack> {
            comp<MudImage> {
                attr.Src data.BlobUrl
                attr.ObjectFit ObjectFit.Cover
                attr.Class "rounded"
                on.load (fun _ -> JS.revokeUrl data.BlobUrl |> Async.StartImmediate)
            }
            comp<MudStack> {
                attr.Row true
                attr.Justify Justify.Center
                comp<MudText> {
                    attr.Color Color.Secondary
                    attr.Typo Typo.subtitle2
                    attr.style "font-family: monospace"
                    $"{data.Length/1024L} KB (%.2f{data.LoadingTime.TotalSeconds}s)"
                }
            }
        }
    | Done (Error text) ->
        comp<MudText> {
            attr.Color Color.Error
            attr.style "font-family: monospace"
            text
        }
