module MyBoleroApp.NotFound

open Bolero.Html
open MudBlazor
open Bolero.MudBlazor

let render () =
    comp<MudText> { "Not found" }
