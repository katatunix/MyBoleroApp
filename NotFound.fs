module MyBoleroApp.NotFound

open Bolero.Html
open MudBlazor
open BudBlazor

let render () =
    comp<MudText> {
        attr.Color Color.Error
        attr.style "font-family: monospace"
        "Oops, that page is gone."
    }
