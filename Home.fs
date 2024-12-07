module MyBoleroApp.Home

open Bolero.Html
open MudBlazor
open Bolero.MudBlazor

let render () =
    comp<MudStack> {
        for _ = 1 to 10 do
            comp<MudPaper> {
                attr.Class "pa-4"
                comp<MudText> {
                    attr.style "font-family: monospace"
                    System.Guid.NewGuid().ToString()
                }
            }
    }
