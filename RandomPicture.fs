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
    let index = random.Next 1000
    let m, cmd = Image.init (makeUrl index)
    { Index = index; Image = m },
    cmd |> Cmd.map ImageMsg

let update msg model =
    match msg with
    | Next | Prev when model.Image.IsLoading ->
        model, Cmd.none
    | Next | Prev ->
        let index = model.Index + (if msg = Next then 1 else -1)
        let m, cmd =
            model.Image |> Image.update (Image.Msg.StartLoad (makeUrl index))
        { model with Index = index; Image = m },
        cmd |> Cmd.map ImageMsg

    | ImageMsg msg ->
        let m, cmd = model.Image |> Image.update msg
        { model with Image = m }, cmd |> Cmd.map ImageMsg

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
                    $"{data.SizeInBytes/1024L} KB (%.2f{data.LoadingTime.TotalSeconds}s)"
                }
            }
        | None ->
            Html.empty ()

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
                attr.style "font-family: monospace"
                attr.Typo Typo.subtitle2
                attr.Color Color.Success
                string model.Index
            }
            button "Next" Next
        }
    }
