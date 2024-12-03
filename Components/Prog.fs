namespace MyBoleroApp.Components

open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Bolero.Html
open MudBlazor

type Prog() =
    inherit ProgramComponent<string,string>()

    [<Parameter>]
    member val Who = "" with get, set

    override this.Program =
        let init _ = this.Who
        let update model msg = model
        let view (model: string) dispatch =
            comp<MudText> { model }
        Program.mkSimple init update view
