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

let init () =
    let tmp =
        [|
            for _ = 1 to 100 do
                let url = $"https://picsum.photos/id/{random.Next(500)}/{width}/{height}"
                Components.Image.init url
        |]
    let model = { Images = tmp |> Array.map fst }
    let cmd =
        tmp
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
            Components.Image.render None true m
    }
