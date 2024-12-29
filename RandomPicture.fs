module MyBoleroApp.RandomPicture

open Elmish
open Bolero
open Bolero.Html
open MudBlazor
open MyBoleroApp.Components

type Model =
    { index: int
      image: Image.Model }

type Msg =
    | Next
    | Prev
    | ImageMsg of Image.Msg

let private width, height = 1500, 850
let private ratio = float(width) / float(height)

let private makeUrl (index: int) =
    $"https://picsum.photos/id/{index}/{width}/{height}"

let init () =
    let index = random.Next 1000
    let m, cmd = Image.init (makeUrl index) (Some ratio)
    { index = index; image = m },
    cmd |> Cmd.map ImageMsg

let update msg model =
    match msg with
    | Next | Prev when model.image.IsLoading ->
        model, Cmd.none
    | Next | Prev ->
        let index = model.index + (if msg = Next then 1 else -1)
        let m, cmd =
            model.image |> Image.update (Image.Msg.StartLoad (makeUrl index))
        { model with index = index; image = m },
        cmd |> Cmd.map ImageMsg

    | ImageMsg msg ->
        let m, cmd = model.image |> Image.update msg
        { model with image = m }, cmd |> Cmd.map ImageMsg

let render model dispatch =
    comp<MudStack> {
        comp<MudStack> {
            attr.style "position: relative"
            Image.render model.image

            match model.image.Data with
            | Some data ->
                let label (text: string) =
                    comp<MudChip<string>> {
                        attr.Color Color.Dark
                        text
                    }
                comp<MudStack> {
                    attr.Row true
                    attr.Spacing 0
                    attr.style "position: absolute;
                                bottom: 5px;
                                right: 5px"
                    label $"{data.sizeInBytes/1024L}KB"
                    label $"%.2f{data.loadingTime.TotalSeconds}s"
                }
            | None ->
                Html.empty()
        }

        let button (text: string) (msg: Msg) =
            comp<MudButton> {
                attr.Variant Variant.Filled
                attr.Color Color.Primary
                attr.Disabled model.image.IsLoading
                on.click (fun _ -> dispatch msg)
                text
            }

        comp<MudStack> {
            attr.Row true
            attr.Justify Justify.Center
            attr.AlignItems AlignItems.Center
            button "Prev" Prev
            comp<MudText> {
                attr.style "font-family: monospace"
                attr.Typo Typo.subtitle2
                attr.Color Color.Success
                string model.index
            }
            button "Next" Next
        }
    }
