module MyBoleroApp.Gallery

open Elmish
open Bolero
open Bolero.Html
open MudBlazor

type Model =
    { images: Components.Image.Model array }

type Msg =
    | ImageMsg of int * Components.Image.Msg
    | Refresh

let [<Literal>] private ImageNumber = 200
let private width, height = 1500, 850
let private ratio = float(width) / float(height)
let private mkUrl () = $"https://picsum.photos/id/{random.Next(500)}/{width}/{height}"

let init js client =
    let arr =
        Array.init ImageNumber (fun _ ->
            let url = mkUrl()
            Components.Image.init js client url (Some ratio)
        )
    let model = { images = arr |> Array.map fst }
    let cmd =
        arr
        |> Seq.mapi (fun index (_, cmd) ->
            cmd |> Cmd.map (fun msg -> ImageMsg (index, msg))
        )
        |> Cmd.batch
    model, cmd

let update js client msg model =
    match msg with
    | ImageMsg (index, msg) ->
        let m, cmd = model.images[index] |> Components.Image.update js client msg
        model.images[index] <- m
        model,
        cmd |> Cmd.map (fun msg -> ImageMsg (index, msg))

    | Refresh ->
        let cmds =
            seq {
                for i = 0 to model.images.Length - 1 do
                    let m = model.images[i]
                    if m.IsLoading |> not then
                        ImageMsg (i, Components.Image.Msg.StartLoad (mkUrl()))
                        |> Cmd.ofMsg
            }
        model, Cmd.batch cmds

let render model dispatch =
    Html.div {
        comp<MudGrid> {
            attr.Spacing 3
            for m in model.images do
                comp<MudItem> {
                    attr.xs 12
                    attr.md 6
                    comp<MudStack> {
                        Components.Image.render m
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
