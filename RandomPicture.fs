module MyBoleroApp.RandomPicture

open Bolero.Html
open Microsoft.AspNetCore.Components.Sections
open MudBlazor
open Bolero.MudBlazor

type Model =
    { Nonce: int }

type Msg =
    | Next

let init () = { Nonce = 0 }

let update msg model =
    match msg with
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
            comp<MudButton> {
                on.click (fun _ -> dispatch Next)
                attr.Variant Variant.Filled
                "Next"
            }
            comp<MudImage> {
                attr.Src $"https://picsum.photos/2000/1400?random={model.Nonce}"
                attr.ObjectFit ObjectFit.Cover
            }
        }
    }
