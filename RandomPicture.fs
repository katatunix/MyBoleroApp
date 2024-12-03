module MyBoleroApp.RandomPicture

open Bolero.Html
open Elmish
open Microsoft.AspNetCore.Components.Sections
open MudBlazor
open Bolero.MudBlazor
open MyBoleroApp.Components

type Model =
    { Image: LoadingImage.Model }

type Msg =
    | Next
    | ImageMsg of LoadingImage.Msg

let [<Literal>] private Url =
    // "https://pic.re/image"
    "https://picsum.photos/2000/1200"

let init () =
    let m, cmd = LoadingImage.init Url
    { Image = m }, cmd |> Cmd.map ImageMsg

let update msg model =
    match msg with
    | Next ->
        let m, cmd = model.Image |> LoadingImage.update LoadingImage.Msg.StartLoad
        { model with Image = m }, cmd |> Cmd.map ImageMsg
    | ImageMsg msg ->
        let m, cmd = model.Image |> LoadingImage.update msg
        { model with Image = m }, cmd |> Cmd.map ImageMsg

let dispose model =
    model.Image |> LoadingImage.dispose

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
            LoadingImage.render model.Image
            comp<MudButton> {
                attr.Variant Variant.Filled
                attr.Color Color.Primary
                attr.disabled model.Image.IsLoading
                on.click (fun _ -> dispatch Next)
                "Next"
            }
        }
    }
