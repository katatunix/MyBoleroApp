namespace MyBoleroApp.Components

open Bolero
open Bolero.Html
open MudBlazor

type Goodbye() =
    inherit ElmishComponent<string,string>()

    override this.View model dispatch =
        comp<MudButton> {
            on.click (fun _ -> dispatch "zzz")
            model
        }
