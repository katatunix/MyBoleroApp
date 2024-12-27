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
    let Src (value: string) = Attr.Make "Src" value
    let ObjectFit (value: ObjectFit) = Attr.Make "ObjectFit" value
    let ButtonType (value: ButtonType) = Attr.Make "ButtonType" value
    let StartIcon (value: string) = Attr.Make "StartIcon" value
    let EndIcon (value: string) = Attr.Make "EndIcon" value
    let Indeterminate (value: bool) = Attr.Make "Indeterminate" value
    let Justify (value: Justify) = Attr.Make "Justify" value
    let Rounded (value: bool) = Attr.Make "Rounded" value
    let Disabled (value: bool) = Attr.Make "Disabled" value
    let Label (value: string) = Attr.Make "Label" value
    let Text (value: string) = Attr.Make "Text" value
    let Value (value: string) = Attr.Make "Value" value
    let ValueInt (value: int) = Attr.Make "Value" value
    let InputType (value: InputType) = Attr.Make "InputType" value
    let Spacing (value: int) = Attr.Make "Spacing" value
    let Fluid (value: bool) = Attr.Make "Fluid" value
    let HideSpinButtons (value: bool) = Attr.Make "HideSpinButtons" value
    let SkeletonType (value: SkeletonType) = Attr.Make "SkeletonType" value
    let FullWidth (value: bool) = Attr.Make "FullWidth" value
    let Square (value: bool) = Attr.Make "Square" value

    let xs (value: int) = Attr.Make "xs" value
    let sm (value: int) = Attr.Make "sm" value
    let md (value: int) = Attr.Make "md" value
    let lg (value: int) = Attr.Make "lg" value
    let xl (value: int) = Attr.Make "xl" value
    let xxl (value: int) = Attr.Make "xxl" value

module on =
    let OpenChanged (action: bool -> unit) = attr.callback "OpenChanged" action
    let ValueChanged action = attr.callback "ValueChanged" action
    let TextChanged action = attr.callback "TextChanged" action
