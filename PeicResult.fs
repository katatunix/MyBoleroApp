module MyBoleroApp.PeicResult

open Bolero
open Bolero.Html
open MudBlazor

let render () =
    let hiddenInput name value =
        input {
            attr.``type`` "hidden"
            attr.name name
            attr.value value
        }

    form {
        attr.action "http://dangkythi.ttngoaingutinhoc.hcm.edu.vn/tra-cuu-diem"
        attr.method "post"
        attr.target "_blank"

        hiddenInput "name"          "BUI PHUONG CHI"
        hiddenInput "birthday"      "03-10-2014"
        hiddenInput "s_examdate"    "16-11-2024"
        hiddenInput "s_capdothi"    "QM"

        let sbd =
            comp<MudNumericField<int>> {
                attr.name "sbd"
                attr.ValueInt 3429
                attr.Label "Student ID"
                attr.Variant Variant.Outlined
                attr.HideSpinButtons true
            }

        let go =
            comp<MudButton> {
                attr.ButtonType ButtonType.Submit
                attr.Variant Variant.Filled
                attr.Color Color.Success
                attr.Size Size.Large
                attr.EndIcon Icons.Material.Filled.RocketLaunch
                attr.FullWidth true
                "Go"
            }

        comp<MudGrid> {
            attr.Spacing 2
            attr.Justify Justify.Center
            let item (x: Node) =
                comp<MudItem> {
                    attr.xs 12
                    attr.sm 8
                    attr.md 6
                    attr.lg 4
                    attr.xl 3
                    x
                }
            item sbd
            comp<MudFlexBreak>
            item go
        }
    }
