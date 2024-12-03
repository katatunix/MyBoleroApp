module MyBoleroApp.RandomPicture

open System.Net.Http
open Bolero.Html
open Elmish
open Microsoft.AspNetCore.Components.Sections
open Microsoft.JSInterop
open MudBlazor
open Bolero.MudBlazor
open MyBoleroApp.Components

type Model =
    { LoadingImage: LoadingImage.Model }

type Msg =
    | Next
    | ImageMsg of LoadingImage.Msg

let [<Literal>] private Url =
    // "https://pic.re/image"
    "https://picsum.photos/2000/1200"

let init (js: IJSRuntime, http: HttpClient) =
    let m, cmd = LoadingImage.init (js, http) Url
    { LoadingImage = m }, cmd |> Cmd.map ImageMsg

let update (js, http) msg model =
    match msg with
    | Next ->
        let m, cmd = model.LoadingImage |> LoadingImage.update (js, http) LoadingImage.Msg.StartLoad
        { model with LoadingImage = m }, cmd |> Cmd.map ImageMsg
    | ImageMsg msg ->
        let m, cmd = model.LoadingImage |> LoadingImage.update (js, http) msg
        { model with LoadingImage = m }, cmd |> Cmd.map ImageMsg

let dispose model =
    model.LoadingImage |> LoadingImage.dispose

let clean msg =
    match msg with
    | ImageMsg msg -> LoadingImage.clean msg
    | _ -> ()

let render model dispatch =
    concat {
        comp<SectionContent> {
            attr.SectionName "Title"
            comp<MudText> {
                attr.Typo Typo.h5
                "Random Picture"
            }
        }
        comp<MudStack> {
            LoadingImage.render model.LoadingImage
            comp<MudButton> {
                attr.Variant Variant.Filled
                attr.Color Color.Primary
                attr.disabled model.LoadingImage.State.IsLoading
                // attr.style "margin: auto"
                on.click (fun _ -> dispatch Next)
                "Next"
            }
        }
    }
