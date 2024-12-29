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

module private Storage =
    let private key = "count"

    let save js count =
        count |> Js.LocalStorage.set js key |> Async.StartImmediate

    let load js =
        async {
            match! Js.LocalStorage.get js key with
            | Int number -> return number
            | _ -> return 0
        }

let init js =
    Loading,
    Cmd.OfAsync.perform Storage.load js EndLoad

let update js msg model =
    match msg, model with
    | EndLoad count, Loading -> Done count
    | (Increase | Decrease), Done count ->
        let count = count + (if msg = Increase then 1 else -1)
        Storage.save js count
        Done count
    | _ -> model

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
