module MyBoleroApp.Counter

open System
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

    let save value =
        value |> Js.LocalStorage.set key |> Async.StartImmediate

    let load () =
        async {
            let! value = Js.LocalStorage.get key
            match value |> Int32.TryParse with
            | true, number -> return number
            | _ -> return 0
        }

let init () =
    Loading,
    Cmd.OfAsync.perform Storage.load () EndLoad

let update msg model =
    let model =
        match msg, model with
        | EndLoad count, Loading -> Done count
        | Increase, Done count -> Done (count + 1)
        | Decrease, Done count -> Done (count - 1)
        | _ -> model

    match model with Done count -> Storage.save count | _ -> ()

    let intent = if random.Next() % 50 = 25 then NavigateToHome else Nope

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
