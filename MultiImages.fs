module MyBoleroApp.MultiImages

open Elmish
open Bolero
open Bolero.Html
open MudBlazor

type Model =
    { Images: Components.Image.Model[] }

type Msg =
    | ImageMsg of int * Components.Image.Msg

let private width, height = 1500, 850
let private ratio = float(width) / float(height)

let init () =
    let arr =
        Array.init 100 (fun _ ->
            let url = $"https://picsum.photos/id/{random.Next(500)}/{width}/{height}"
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

let update (msg: Msg) (model: Model) =
    match msg with
    | ImageMsg (index, msg) ->
        let m, cmd = model.Images[index] |> Components.Image.update msg
        model.Images[index] <- m
        model,
        cmd |> Cmd.map (fun msg -> ImageMsg (index, msg))

let render (model: Model) =
    comp<MudStack> {
        for m in model.Images do
            Components.Image.render m
    }
