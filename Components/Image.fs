module MyBoleroApp.Components.Image

open System
open Elmish
open Bolero
open Bolero.Html
open MudBlazor
open MyBoleroApp

type Data =
    { BlobUrl: string
      SizeInBytes: int64
      LoadingTime: TimeSpan }

type State =
    | Loading of Guid
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
    | EndLoad of Guid * Result<Data, string>

let private loadCmd guid (imageUrl: string) =
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
        (fun data -> EndLoad (guid, Ok data))
        (fun ex -> EndLoad (guid, Error ex.Message))

let init url =
    let guid = Guid.NewGuid()
    { Url = url
      State = Loading guid },
    loadCmd guid url

let private revoke = function
    | Ok data -> Js.revokeUrl data.BlobUrl |> Async.StartImmediate
    | _ -> ()

let update msg model =
    match msg, model.State with
    | StartLoad _, Loading _ ->
        model, Cmd.none

    | StartLoad url, Done result ->
        revoke result
        let guid = Guid.NewGuid()
        { model with Url = url; State = Loading guid },
        loadCmd guid url

    | EndLoad (guid, result), Loading guid' when guid = guid' ->
        { model with State = Done result }, Cmd.none

    | EndLoad (_, result), _ ->
        revoke result
        model, Cmd.none

let dispose model =
    match model.State with
    | Done result -> revoke result
    | _ -> ()

let clean msg =
    match msg with
    | EndLoad (_, result) -> revoke result
    | _ -> ()

let render model =
    match model.State with
    | Loading _ ->
        comp<MudProgressLinear> {
            attr.Indeterminate true
            attr.Color Color.Primary
            attr.Size Size.Medium
            attr.Rounded true
        }
    | Done (Ok data) ->
        comp<MudImage> {
            attr.Src data.BlobUrl
            attr.ObjectFit ObjectFit.Cover
            attr.Class "rounded"
        }
    | Done (Error str) ->
        comp<MudText> {
            attr.Color Color.Error
            attr.style "font-family: monospace"
            str
        }
