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
      ratio: float option
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

let private loadCmd js client imageUrl =
    Cmd.OfAsync.either
        (fun imageUrl -> async {
            let start = DateTime.Now
            use! response = Http.getStream client imageUrl
            let length = response.Stream.Length
            let! blobUrl = Js.createUrl js response.Stream response.ContentType
            return { blobUrl = blobUrl
                     sizeInBytes = length
                     loadingTime = DateTime.Now - start }
        })
        imageUrl
        (fun data -> EndLoad (Ok data))
        (fun ex -> EndLoad (Error ex.Message))

let init js client url ratio =
    { url = url
      ratio = ratio
      state = Loading },
    loadCmd js client url

let private dispose = function
    | Ok data -> (data.blobUrl: IDisposable).Dispose()
    | _ -> ()

let update js client msg model =
    match msg, model.state with
    | StartLoad _, Loading ->
        model, Cmd.none

    | StartLoad url, Done result ->
        dispose result
        { model with url = url; state = Loading },
        loadCmd js client url

    | EndLoad result, Loading ->
        { model with state = Done result }, Cmd.none

    | EndLoad result, _ ->
        dispose result
        model, Cmd.none

let render model =
    let attrRatio () =
        match model.ratio with
        | Some ratio ->
            attr.style $"height: auto; aspect-ratio: {ratio}"
        | None ->
            attr.empty ()

    match model.state with
    | Loading ->
        comp<MudSkeleton> {
            attr.SkeletonType SkeletonType.Rectangle
            attrRatio ()
        }

    | Done (Ok data) ->
        comp<MudImage> {
            attr.Src data.blobUrl.Value
            attrRatio ()
        }

    | Done (Error _msg) ->
        comp<MudPaper> {
            attr.Square true
            attr.Class "mud-error"
            attrRatio ()
        }
