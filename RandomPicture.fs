module MyBoleroApp.RandomPicture

open Bolero.Html
open Microsoft.AspNetCore.Components.Sections
open MudBlazor
open Bolero.MudBlazor

type Model =
    { Nonce: int }

type Msg =
    | Prev
    | Next


let init () = { Nonce = 0 }

let update msg model =
    match msg with
    | Prev -> { model with Nonce = model.Nonce - 1 }
    | Next -> { model with Nonce = model.Nonce + 1 }

let render model dispatch =
    concat {
        comp<SectionContent> {
            attr.SectionName "Title"
            comp<MudText> {
                attr.Typo Typo.h5
                "Random Picture"
            }
        }
        comp<MudStack> {
            comp<MudImage> {
                attr.Src $"https://picsum.photos/2000/1200?random={model.Nonce}"
                attr.ObjectFit ObjectFit.Cover
                attr.Class "rounded-lg"
            }
            comp<MudButtonGroup> {
                attr.Variant Variant.Outlined
                attr.Color Color.Primary
                attr.style "margin:auto"
                comp<MudButton> {
                    on.click (fun _ -> dispatch Prev)
                    "Prev"
                }
                comp<MudButton> {
                    on.click (fun _ -> dispatch Next)
                    "Next"
                }
            }
        }
    }
