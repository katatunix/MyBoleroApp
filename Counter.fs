module MyBoleroApp.Counter

open Bolero.Html
open MudBlazor
open Bolero.MudBlazor

type Model =
    { Count: int }

type Msg =
    | Increase
    | Decrease

type Intent =
    | Nope
    | NavigateToHome

let init () =
    { Count = 0 }

let update msg model =
    let model =
        match msg with
        | Increase -> { model with Count = model.Count + 1 }
        | Decrease -> { model with Count = model.Count - 1 }

    let intent = if random.Next() % 50 = 25 then NavigateToHome else Nope

    model, intent

let render model dispatch =
    comp<MudStack> {
        comp<MudText> {
            attr.Typo Typo.h4
            attr.Color Color.Inherit
            attr.style "font-family: monospace"
            string model.Count
        }
        comp<MudStack> {
            attr.Row true
            attr.Justify Justify.FlexStart

            let button (text: string) color msg =
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
