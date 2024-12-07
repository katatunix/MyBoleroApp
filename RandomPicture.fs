module MyBoleroApp.RandomPicture

open Elmish
open Bolero
open Bolero.Html
open MudBlazor
open MyBoleroApp.Components

type Model =
    { Index: int
      Image: Image.Model }

type Msg =
    | Next
    | Prev
    | ImageMsg of Image.Msg

let private width, height = 1500, 850

let private makeUrl (index: int) =
    $"https://picsum.photos/id/{index}/{width}/{height}"

let init () =
    let index = 250 + (random.Next 500)
    let imageModel, imageCmd = Image.init (makeUrl index)
    { Index = index; Image = imageModel },
    imageCmd |> Cmd.map ImageMsg

let update msg model =
    match msg with
    | Next | Prev when model.Image.IsLoading ->
        model, Cmd.none
    | Next | Prev ->
        let index = model.Index + (if msg = Next then 1 else -1)
        let imageModel, imageCmd =
            model.Image |> Image.update (Image.Msg.StartLoad (makeUrl index))
        { model with Index = index; Image = imageModel },
        imageCmd |> Cmd.map ImageMsg

    | ImageMsg msg ->
        let imageModel, imageCmd = model.Image |> Image.update msg
        { model with Image = imageModel },
        imageCmd |> Cmd.map ImageMsg

let dispose model =
    model.Image |> Image.dispose

let clean msg =
    match msg with
    | ImageMsg msg -> Image.clean msg
    | _ -> ()

let render model dispatch =
    comp<MudStack> {
        Image.render model.Image

        match model.Image.Data with
        | Some data ->
            comp<MudStack> {
                attr.Row true
                attr.Justify Justify.Center
                comp<MudText> {
                    attr.Color Color.Secondary
                    attr.Typo Typo.subtitle2
                    attr.style "font-family: monospace"
                    $"{data.SizeInBytes/1024L}KB (%.2f{data.LoadingTime.TotalSeconds}s)"
                }
            }
        | None ->
            Html.empty()

        let button (text: string) msg =
            comp<MudButton> {
                attr.Variant Variant.Filled
                attr.Color Color.Primary
                attr.Disabled model.Image.IsLoading
                on.click (fun _ -> dispatch msg)
                text
            }

        comp<MudStack> {
            attr.Row true
            attr.Justify Justify.Center
            attr.AlignItems AlignItems.Center
            button "Prev" Prev
            comp<MudText> {
                attr.Color Color.Primary
                attr.style "font-family: monospace"
                attr.Typo Typo.subtitle2
                string model.Index
            }
            button "Next" Next
        }
    }
