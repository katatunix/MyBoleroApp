module MyBoleroApp.RandomPicture

open Bolero.Html
open Elmish
open Microsoft.AspNetCore.Components.Sections
open MudBlazor
open Bolero.MudBlazor
open MyBoleroApp.Components

type Model =
    { Seed: int
      Image: LoadingImage.Model
    }

type Msg =
    | Next
    | Prev
    | ImageMsg of LoadingImage.Msg

let makeUrl (seed: int) =
    $"https://picsum.photos/seed/{seed}/2000/1200"

let init () =
    let seed = Common.random.Next(1000000)
    let m, cmd = LoadingImage.init (makeUrl seed)
    { Seed = seed; Image = m }, cmd |> Cmd.map ImageMsg

let update msg model =
    match msg with
    | Next | Prev ->
        let seed = model.Seed + (if msg = Next then 1 else -1)
        let m, cmd = model.Image |> LoadingImage.update (LoadingImage.Msg.StartLoad (makeUrl seed))
        { model with Seed = seed; Image = m }, cmd |> Cmd.map ImageMsg

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
            comp<MudStack> {
                attr.Row true
                attr.Justify Justify.Center
                comp<MudButton> {
                    attr.Variant Variant.Filled
                    attr.Color Color.Primary
                    attr.disabled model.Image.IsLoading
                    on.click (fun _ -> dispatch Prev)
                    "Prev"
                }
                comp<MudButton> {
                    attr.Variant Variant.Filled
                    attr.Color Color.Primary
                    attr.disabled model.Image.IsLoading
                    on.click (fun _ -> dispatch Next)
                    "Next"
                }
            }
        }
    }
