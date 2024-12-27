module MyBoleroApp.Counter

open Elmish
open Bolero
open Bolero.Html
open MudBlazor

type Model =
    | Loading
    | Done of count: int

type Msg =
    | EndLoad of count: int
    | Increase
    | Decrease

type Intent =
    | Nope
    | NavigateToHome

module private Storage =
    let private key = "count"

    let save count =
        count |> Js.LocalStorage.set key |> Async.StartImmediate

    let load () =
        async {
            match! Js.LocalStorage.get key with
            | Int number -> return number
            | _ -> return 0
        }

let init () =
    Loading,
    Cmd.OfAsync.perform Storage.load () EndLoad

let update msg model =
    let model =
        match msg, model with
        | EndLoad count, Loading -> Done count
        | (Increase | Decrease), Done count ->
            let count = count + (if msg = Increase then 1 else -1)
            Storage.save count
            Done count
        | _ -> model

    let intent =
        Nope
        // if random.Next() % 2 = 0 then NavigateToHome else Nope

    model, intent

let render model dispatch =
    match model with
    | Loading ->
        Html.empty()

    | Done count ->
        comp<MudStack> {
            comp<MudText> {
                attr.Typo Typo.h4
                attr.Color Color.Inherit
                attr.style "font-family: monospace"
                string count
            }

            comp<MudStack> {
                attr.Row true
                attr.Justify Justify.FlexStart

                let button (text: string) (color: Color) (msg: Msg) =
                    comp<MudButton> {
                        attr.Variant Variant.Filled
                        attr.Color color
                        attr.Disabled false
                        on.click (fun _ -> dispatch msg)
                        text
                    }

                button "Increase" Color.Primary Increase
                button "Decrease" Color.Secondary Decrease
            }
        }
