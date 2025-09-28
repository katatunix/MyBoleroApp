namespace MyBoleroApp.Components

open Microsoft.AspNetCore.Components
open Bolero
open Bolero.Html
open MudBlazor

type Hello() =
    inherit Component()

    [<Parameter>]
    member val Who = "" with get, set

    override this.Render() =
        comp<MudText> {
            $"Hello: {this.Who}"
        }
