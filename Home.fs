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
        comp<MudStack> {
            for _ = 1 to 100 do
            comp<MudPaper> {
                attr.Class "pa-4"
                comp<MudText> {
                    attr.style "font-family:monospace"
                    System.Guid.NewGuid().ToString()
                }
            }
        }
    }
