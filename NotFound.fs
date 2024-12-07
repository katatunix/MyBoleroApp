module MyBoleroApp.NotFound

open Bolero.Html
open MudBlazor
open Bolero.MudBlazor

let render () =
    comp<MudText> {
        attr.Color Color.Error
        attr.style "font-family: monospace"
        "Oops, that page is gone."
    }
