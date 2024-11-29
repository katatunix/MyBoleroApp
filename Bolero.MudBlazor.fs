module Bolero.MudBlazor

open Bolero.Html
open Microsoft.AspNetCore.Components.Routing
open MudBlazor

module attr =
    let Color (value: Color) = Attr.Make "Color" value
    let Icon (value: string) = Attr.Make "Icon" value
    let Edge (value: Edge) = Attr.Make "Edge" value
    let Open (value: bool) = Attr.Make "Open" value
    let Elevation (value: int) = Attr.Make "Elevation" value
    let SectionName (value: string) = Attr.Make "SectionName" value
    let Variant (value: Variant) = Attr.Make "Variant" value
    let Typo (value: Typo) = Attr.Make "Typo" value
    let Class (value: string) = Attr.Make "Class" value
    let Href (value: string) = Attr.Make "Href" value
    let Match (value: NavLinkMatch) = Attr.Make "Match" value
    let Row (value: bool) = Attr.Make "Row" value
    let IsDarkMode (value: bool) = Attr.Make "IsDarkMode" value
    let MaxWidth (value: MaxWidth) = Attr.Make "MaxWidth" value
    let AlignItems (value: AlignItems) = Attr.Make "AlignItems" value
    let Size (value: Size) = Attr.Make "Size" value

module on =
    let OpenChanged (action: bool -> unit) = attr.callback "OpenChanged" action
    let ValueChanged action = attr.callback "ValueChanged" action
