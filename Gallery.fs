module MyBoleroApp.Gallery

open Elmish
open Bolero.Html
open MudBlazor
open BudBlazor
open MyBoleroApp.Components

type Model =
    { imageModels: Image.Model[] }

type Msg =
    | ImageMsg of int * Image.Msg
    | Refresh

let [<Literal>] private ImageNumber = 200
let private width, height = 1500, 850
let private ratio = float(width) / float(height)
let private randomUrl () = $"https://picsum.photos/id/{random.Next(500)}/{width}/{height}"

let init jsRuntime httpClient =
    let arr =
        Array.init ImageNumber (fun _ ->
            let url = randomUrl ()
            Image.init jsRuntime httpClient url (Some ratio)
        )
    let model = { imageModels = arr |> Array.map fst }
    let cmd =
        arr
        |> Seq.mapi (fun index (_, cmd) ->
            cmd |> Cmd.map (fun msg -> ImageMsg (index, msg))
        )
        |> Cmd.batch
    model, cmd

let update jsRuntime httpClient msg model =
    match msg with
    | ImageMsg (index, msg) ->
        let imageModel, cmd = model.imageModels[index] |> Image.update jsRuntime httpClient msg
        model.imageModels[index] <- imageModel
        let cmd = cmd |> Cmd.map (fun msg -> ImageMsg (index, msg))
        model, cmd

    | Refresh ->
        let cmds = seq {
            for i = 0 to model.imageModels.Length - 1 do
                let m = model.imageModels[i]
                if m.IsLoading |> not then
                    ImageMsg (i, Image.Msg.StartLoad (randomUrl()))
                    |> Cmd.ofMsg
        }
        model, Cmd.batch cmds

let render model dispatch =
    div {
        comp<MudGrid> {
            attr.Spacing 3
            for imageModel in model.imageModels do
                comp<MudItem> {
                    attr.xs 12
                    attr.md 6
                    comp<MudStack> {
                        Image.render imageModel
                    }
                }
        }
        comp<MudFab> {
            attr.StartIcon Icons.Material.Filled.Adjust
            attr.Color Color.Secondary
            attr.style "position: fixed; bottom: 16px; right: 16px"
            on.click (fun _ -> dispatch Refresh)
        }
    }
