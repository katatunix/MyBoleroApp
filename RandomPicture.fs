module MyBoleroApp.RandomPicture

open Elmish
open Bolero.Html
open MudBlazor
open BudBlazor
open MyBoleroApp.Components

type Model =
    { index: int
      imageModel: Image.Model }

type Msg =
    | Next
    | Prev
    | ImageMsg of Image.Msg

let private width, height = 1500, 850
let private ratio = float(width) / float(height)

let private makeUrl (index: int) =
    $"https://picsum.photos/id/{index}/{width}/{height}"

let init jsRuntime httpClient =
    let index = random.Next 1000
    let m, cmd = Image.init jsRuntime httpClient (makeUrl index) (Some ratio)
    { index = index; imageModel = m },
    cmd |> Cmd.map ImageMsg

let update jsRuntime httpClient msg model =
    match msg with
    | Next | Prev when model.imageModel.IsLoading ->
        model, Cmd.none
    | Next | Prev ->
        let index = model.index + (if msg = Next then 1 else -1)
        let m, cmd =
            model.imageModel |> Image.update jsRuntime httpClient (Image.Msg.StartLoad (makeUrl index))
        { model with index = index; imageModel = m },
        cmd |> Cmd.map ImageMsg

    | ImageMsg msg ->
        let m, cmd = model.imageModel |> Image.update jsRuntime httpClient msg
        { model with imageModel = m }, cmd |> Cmd.map ImageMsg

let render model dispatch =
    comp<MudStack> {
        comp<MudStack> {
            attr.style "position: relative"
            Image.render model.imageModel
            match model.imageModel.Data with
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
                empty ()
        }

        let button (text: string) (msg: Msg) =
            comp<MudButton> {
                attr.Variant Variant.Filled
                attr.Color Color.Primary
                attr.Disabled model.imageModel.IsLoading
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
