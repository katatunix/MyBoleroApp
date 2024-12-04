module MyBoleroApp.Counter

open Microsoft.AspNetCore.Components.Sections
open Bolero.Html
open MudBlazor
open Bolero.MudBlazor

type Model =
    { Count: int }

type Msg =
    | Inc
    | Dec

type Intent =
    | Nope
    | NavigateToHome

let init (start: int option) =
    { Count = start |> Option.defaultValue 0 }

let update msg model =
    let model =
        match msg with
        | Inc -> { model with Count = model.Count + 1 }
        | Dec -> { model with Count = model.Count - 1 }

    let intent =
        if Common.random.Next() % 50 = 0
        then NavigateToHome
        else Nope

    model, intent

let render model dispatch =
    concat {
        comp<SectionContent> {
            attr.SectionName "Title"
            comp<MudText> {
                attr.Typo Typo.h5
                "Counter"
            }
        }
        comp<MudStack> {
            comp<MudText> {
                attr.Typo Typo.h4
                attr.Color Color.Inherit
                attr.style "font-family:monospace"
                string model.Count
            }
            comp<MudStack> {
                attr.Row true
                attr.Justify Justify.FlexStart

                let btn (text: string) color msg =
                    comp<MudButton> {
                        attr.Variant Variant.Filled
                        attr.Color color
                        attr.Disabled false
                        on.click (fun _ -> dispatch msg)
                        text
                    }

                btn "Increase" Color.Primary Inc
                btn "Decrease" Color.Secondary Dec
            }
        }
    }
