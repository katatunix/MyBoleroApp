module MyBoleroApp.NotFound

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
                "Not Found"
            }
        }
        comp<MudText> { "Not found" }
    }
