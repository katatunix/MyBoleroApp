module MyBoleroApp.PeicResult

open Bolero.Html
open MudBlazor
open Bolero.MudBlazor

let render () =
    let hiddenInput name value =
        input {
            attr.``type`` "hidden"
            attr.name name
            attr.value value
        }

    form {
        attr.action "http://dangkythi.ttngoaingutinhoc.hcm.edu.vn/tra-cuu-diem"
        attr.method "POST"
        attr.target "_blank"

        hiddenInput "name"          "BUI PHUONG CHI"
        hiddenInput "birthday"      "03-10-2014"
        hiddenInput "sbd"           "3429"
        hiddenInput "s_examdate"    "16-11-2024"
        hiddenInput "s_capdothi"    "QM"

        comp<MudButton> {
            attr.ButtonType ButtonType.Submit
            attr.Variant Variant.Filled
            attr.Color Color.Success
            attr.Size Size.Large
            attr.EndIcon Icons.Material.Filled.Send
            "Get Result"
        }
    }
