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

let render homeUrl model dispatch =
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
                string model.Count
            }
            comp<MudStack> {
                attr.Row true
                comp<MudButton> {
                    attr.Variant Variant.Filled
                    attr.Color Color.Primary
                    on.click (fun _ -> dispatch Inc)
                    "Increase"
                }
                comp<MudButton> {
                    attr.Variant Variant.Filled
                    attr.Color Color.Secondary
                    on.click (fun _ -> dispatch Dec)
                    "Decrease"
                }
            }
            // comp<MudLink> {
            //     attr.Href homeUrl
            //     "Back to Home"
            // }
        }
    }
