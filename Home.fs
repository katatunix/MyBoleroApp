module MyBoleroApp.Home

open Bolero.Html
open Microsoft.AspNetCore.Components.Sections
open MudBlazor
open Bolero.MudBlazor

let render () =
    concat {
        comp<SectionContent> {
            attr.SectionName "Title"
            comp<MudText> {
                attr.Typo Typo.h5
                "Home"
            }
        }
        comp<MudText> { attr.Typo Typo.h4; "Welcome to the hell" }
    }
