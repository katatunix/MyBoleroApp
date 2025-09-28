module MyBoleroApp.Home

open Bolero.Html
open MudBlazor
open BudBlazor

let render () =
    let paper () =
        comp<MudPaper> {
            attr.Class "pa-4"
            comp<MudText> {
                attr.style "font-family: monospace"
                System.Guid.NewGuid().ToString()
            }
        }

    comp<MudGrid> {
        for _ = 1 to 12 do
            comp<MudItem> {
                attr.xs 12
                attr.md 6
                attr.lg 4
                attr.xl 3
                paper()
            }
    }
