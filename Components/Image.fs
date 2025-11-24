module MyBoleroApp.Components.Image

open System
open Bolero
open Elmish
open Bolero.Html
open MudBlazor
open BudBlazor
open MyBoleroApp

type Data = {
    blobUrl: Js.URL
    sizeInBytes: int64
    loadingTime: TimeSpan
}

type State =
    | Loading
    | Done of Data option

type Model = {
    ratio: float option
    state: State
} with
    member this.IsLoading =
        this.state.IsLoading

    member this.Data =
        match this.state with
        | Done data -> data
        | _ -> None

type Msg =
    | StartLoad of url: string
    | EndLoad of Data option

let private loadCmd jsRuntime httpClient imageUrl =
    Cmd.OfAsync.either
        (fun imageUrl -> async {
            let start = DateTime.Now
            use! response = Http.getStream httpClient imageUrl
            let length = response.Stream.Length
            let! blobUrl = Js.createUrl jsRuntime response.Stream response.ContentType
            return {
                blobUrl = blobUrl
                sizeInBytes = length
                loadingTime = DateTime.Now - start
            }
        })
        imageUrl
        (Some >> EndLoad)
        (fun _ -> EndLoad None)

let init jsRuntime httpClient url ratio =
    let model = {
        ratio = ratio
        state = Loading
    }
    let cmd = loadCmd jsRuntime httpClient url
    model, cmd

let private dispose = function
    | Some data -> (data.blobUrl: IDisposable).Dispose()
    | None -> ()

let update jsRuntime httpClient msg model =
    match msg, model.state with
    | StartLoad _, Loading ->
        model, Cmd.none

    | StartLoad url, Done result ->
        dispose result
        { model with state = Loading },
        loadCmd jsRuntime httpClient url

    | EndLoad result, Loading ->
        { model with state = Done result }, Cmd.none

    | EndLoad result, _ ->
        dispose result
        model, Cmd.none

type private Component() =
    inherit ElmishComponent<Model,Msg>()

    override this.ShouldRender(oldModel, newModel) =
        oldModel.state <> newModel.state

    override this.View model _ =
        let commonAttr =
            match model.ratio with
            | Some ratio ->
                attr.style $"height: auto; aspect-ratio: {ratio}"
            | None ->
                attr.empty()

        match model.state with
        | Loading ->
            comp<MudSkeleton> {
                attr.SkeletonType SkeletonType.Rectangle
                commonAttr
            }

        | Done (Some data) ->
            comp<MudImage> {
                attr.Src data.blobUrl.Value
                commonAttr
            }

        | Done None ->
            comp<MudPaper> {
                attr.Square true
                attr.Class "mud-error"
                commonAttr
            }

let render model =
    ecomp<Component,_,_> model ignore { attr.empty() }
