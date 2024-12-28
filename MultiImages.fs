module MyBoleroApp.MultiImages

open Elmish
open Bolero
open Bolero.Html
open MudBlazor

type Model =
    { Images: Components.Image.Model array }

type Msg =
    | ImageMsg of int * Components.Image.Msg
    | Refresh

let [<Literal>] private ImageNumber = 200
let private width, height = 1500, 850
let private ratio = float(width) / float(height)
let private mkUrl () = $"https://picsum.photos/id/{random.Next(500)}/{width}/{height}"

let init () =
    let arr =
        Array.init ImageNumber (fun _ ->
            let url = mkUrl()
            Components.Image.init url (Some ratio)
        )
    let model = { Images = arr |> Array.map fst }
    let cmd =
        arr
        |> Seq.mapi (fun index (_, cmd) ->
            cmd |> Cmd.map (fun msg -> ImageMsg (index, msg))
        )
        |> Cmd.batch
    model, cmd

let update msg model =
    match msg with
    | ImageMsg (index, msg) ->
        let m, cmd = model.Images[index] |> Components.Image.update msg
        model.Images[index] <- m
        model, cmd |> Cmd.map (fun msg -> ImageMsg (index, msg))

    | Refresh ->
        let cmds =
            seq {
                for i = 0 to model.Images.Length - 1 do
                    let m = model.Images[i]
                    if m.IsLoading |> not then
                        ImageMsg (i, Components.Image.Msg.StartLoad (mkUrl()))
                        |> Cmd.ofMsg
            }
        model, Cmd.batch cmds

let render model dispatch =
    Html.div {
        comp<MudStack> {
            for m in model.Images do
                Components.Image.render m
        }
        comp<MudFab> {
            attr.StartIcon Icons.Material.Filled.Adjust
            attr.Color Color.Secondary
            attr.style "position: fixed; bottom: 16px; right: 16px"
            on.click (fun _ -> dispatch Refresh)
        }
    }
